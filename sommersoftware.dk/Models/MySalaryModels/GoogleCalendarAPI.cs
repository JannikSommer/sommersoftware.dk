using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using Google.Apis.Auth.AspNetCore3;

namespace sommersoftware.dk.Models.MySalaryModels
{
    public class GoogleCalendarAPI
    {
        private SettingsModel UserSettings;
        private WageDetailsModel WageDetails;
        
        public async Task<List<YearModel>> GetYears(SettingsModel userSettings, CalendarService service) 
        {
            UserSettings = userSettings;
            WageDetails = new WageDetailsModel(UserSettings.HourWage, UserSettings.EveningWage, UserSettings.SaturdayWage, UserSettings.SundayWage);
            
            EventsResource.ListRequest request;
            if (UserSettings.CalendarLink.Length > 0)
            {
                request = service.Events.List(UserSettings.CalendarLink);
            }
            else
            {
                request = service.Events.List("primary");
            }
            request.TimeMin = UserSettings.BeginDate;
            request.TimeMax = UserSettings.EndDate;
            request.MaxResults = (UserSettings.EndDate - UserSettings.BeginDate).Days;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events shiftEvents = await request.ExecuteAsync();
            Events holidayEvents = await GetDanishHolidaysAsync(service, UserSettings.EndDate);

            return ExtractShifts(shiftEvents, holidayEvents);
        }

        private Events GetDanishHolidays(CalendarService service, DateTime lastEventDate)
        {
            EventsResource.ListRequest request = service.Events.List("da.danish#holiday@group.v.calendar.google.com");
            request.TimeMin = UserSettings.BeginDate;
            request.TimeMax = UserSettings.EndDate;
            request.MaxResults = (UserSettings.EndDate - UserSettings.BeginDate).Days;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events allEvents = request.Execute();
            RemoveRedundantHolidays(allEvents, lastEventDate);
            return allEvents;
        }

        private async Task<Events> GetDanishHolidaysAsync(CalendarService service, DateTime lastEventDate)
        {
            EventsResource.ListRequest request = service.Events.List("da.danish#holiday@group.v.calendar.google.com");
            request.TimeMin = UserSettings.BeginDate;
            request.TimeMax = UserSettings.EndDate;
            request.MaxResults = (UserSettings.EndDate - UserSettings.BeginDate).Days;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events allEvents = await request.ExecuteAsync();
            RemoveRedundantHolidays(allEvents, lastEventDate);
            return allEvents;
        }

        private void RemoveRedundantHolidays(Events allEvents, DateTime lastEventDate)
        {
            
            foreach (var item in allEvents.Items.ToList()) 
            {
                DateTime itemDate = DateTime.Parse(item.Start.Date);
                if (DateTime.Compare(itemDate, lastEventDate) >= 0)
                {
                    allEvents.Items.Remove(item);
                }
                else
                {
                    string str = item.Summary.ToUpper();
                    if (!(str.Contains("PÅSKEDAG")
                        || str.Contains("JULEDAG")
                        || str.Contains("PINSEDAG")
                        || str.Contains("NYTÅRSDAG")
                        || str.Contains("SKÆRTORSDAG")
                        || str.Contains("LANGFREDAG")
                        || str.Contains("BEDEDAG")
                        || str.Contains("HIMMELFARTSDAG")))
                    {
                        allEvents.Items.Remove(item);
                    }
                }
            }
        }

        private List<YearModel> ExtractShifts(Events events, Events holidayEvents)
        {
            List<YearModel> years = new List<YearModel>();
            List<ShiftParentModel> allShifts = HandleShiftEvents(years, events);
            List<PlanningPeriodModel> planningPeriods = DivideCalendarIntoPlanningPeriods(allShifts); //Used to calculate the amount of holidya reduction for each planning period
            Hashtable holidays = HandleHoldidayEvents(holidayEvents);
            AddHolidayReductionToListOfShifts(years, allShifts, planningPeriods, holidays, holidayEvents);
            return years;
        }

        private void AddHolidayReductionToListOfShifts(List<YearModel> years, List<ShiftParentModel> allShifts, List<PlanningPeriodModel> planningPeriods, Hashtable holidays, Events events)
        {
            int planningPeriondCount = 0;
            int yearIndex = 0;
            int currentYear = 0;
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var item in events.Items)
                {
                    if (item.Start != null)
                    {
                        string when = item.Start.Date.ToString();
                        string[] numbers = Regex.Split(when, @"\D+");
                        DateTime dt = new DateTime(int.Parse(numbers[0]), int.Parse(numbers[1]), int.Parse(numbers[2])); // year, month, day - Gets Weekday by date
                        ShiftParentModel shift = null;
                        if (currentYear == 0)
                        {
                            currentYear = int.Parse(numbers[0]);
                        }
                        if (int.Parse(numbers[1]) == 12 && int.Parse(numbers[2]) > 18)
                        {
                            // Makes sure that holiday credit are added to the right year. 
                            for (int i = 0; i < years.Count; i++)
                            {
                                if (years[i].YearNumber == currentYear + 1)
                                {
                                    yearIndex = i;
                                }
                            }
                        }
                        if (DateTime.Compare(dt, planningPeriods[planningPeriondCount].PeriodEnd) <= 0) 
                        {
                            shift = new HolidayShiftModel(dt, true, planningPeriods[planningPeriondCount].ReductionAmount, UserSettings.HourWage, item.Summary);
                        }
                        else
                        {
                            if (planningPeriondCount + 1 !< planningPeriods.Count)
                            {
                                planningPeriondCount++;
                                while (DateTime.Compare(dt, planningPeriods[planningPeriondCount].PeriodEnd) > 0)
                                {
                                    planningPeriondCount++;
                                }
                                shift = new HolidayShiftModel(dt, true, planningPeriods[planningPeriondCount].ReductionAmount, UserSettings.HourWage, item.Summary);
                            }
                        }
                        if (shift != null)
                        {
                            AddShiftToMonth(int.Parse(numbers[1]), int.Parse(numbers[2]), years, yearIndex, shift);
                        }
                    }
                }
                foreach (YearModel year in years)
                {
                    foreach (MonthModel month in year.Months)
                    {
                        month.Shifts.Sort();
                    }
                }
            }
        }

        private List<ShiftParentModel> HandleShiftEvents(List<YearModel> years, Events events)
        {
            int currentYear = 0; // Baseline initialization - used to give Year-classes a yearnumber.
            int yearCounter = 0; // Baseline initialization - Used to index the List years. 
            string shiftKeyword = UserSettings.ShiftKeyword;
            List<ShiftParentModel> allShifts = new List<ShiftParentModel>();

            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    if (eventItem.Summary != null)
                    {
                        if (eventItem.Summary.ToUpper().Contains(shiftKeyword.ToUpper()))
                        {
                            string when = eventItem.Start.DateTimeRaw.ToString() + " - " + eventItem.End.DateTimeRaw.ToString();
                            
                            string[] numbers = Regex.Split(when, @"\D+");
                            if (currentYear == 0)
                            {
                                currentYear = int.Parse(numbers[0]);
                                years.Add(new YearModel(currentYear, UserSettings.TotalTaxPercentage, UserSettings.TotalTaxDeduction, WageDetails));
                            }
                            if (currentYear == int.Parse(numbers[0]))
                            {
                                years[yearCounter].YearNumber = currentYear;
                                if (int.Parse(numbers[1]) == 12 && int.Parse(numbers[2]) > 18)
                                {
                                    yearCounter++;
                                    years.Add(new YearModel(currentYear + 1, UserSettings.TotalTaxPercentage, UserSettings.TotalTaxDeduction, WageDetails));
                                    currentYear = int.Parse(numbers[0]) + 1;
                                }
                            }
                            try 
                            {
                                CultureInfo cultureInfo = new CultureInfo("da-DK");
                                string dateStr = int.Parse(numbers[2]) + "/" + int.Parse(numbers[1]) + "/" + int.Parse(numbers[0]);
                                DateTime date = Convert.ToDateTime(dateStr, cultureInfo);
                                ShiftParentModel shift = new ShiftModel(date, date.DayOfWeek.ToString(), WeekNumber((DateTime)eventItem.Start.DateTime), int.Parse(numbers[0]),
                                                                  int.Parse(numbers[3]), int.Parse(numbers[4]), int.Parse(numbers[11]), int.Parse(numbers[12]), WageDetails);

                                AddShiftToMonth(int.Parse(numbers[1]), int.Parse(numbers[2]), years, yearCounter, shift);
                                allShifts.Add(shift);
                            }
                            catch (Exception)
                            {
                                // For what ever reason, we should not crash but use a generic exception
                                continue;
                            }
                        }
                    }
                }
            }
            return allShifts;
        }

        private void AddShiftToMonth(int monthNumber, int dateNumber, List<YearModel> years, int yearCounter, ShiftParentModel shift)
        {
            if (dateNumber < 19)
            {
                //if (monthNumber == 0)
                //    years[yearCounter - 1].Months[monthNumber].Shifts.Add(shift);
                //else
                
                years[yearCounter].Months[monthNumber - 1].Shifts.Add(shift);
            }
            else
            {
                if (monthNumber == 12)
                    years[yearCounter].Months[0].Shifts.Add(shift); // Adds to the first month of the following year
                
                else
                    years[yearCounter].Months[monthNumber].Shifts.Add(shift); // adds to next month in current year
            }
        }

        private Hashtable HandleHoldidayEvents(Events events)
        {
            // adds every item from holidays into a hashtable for easier look-up later
            Hashtable holidays = new Hashtable();
            foreach (var item in events.Items)
            {
                holidays.Add(item, item.Start);
            }
            return holidays;
        }
        
        private List<PlanningPeriodModel> DivideCalendarIntoPlanningPeriods(List<ShiftParentModel> shifts)
        {
            List<YearModel> years = DivideShiftsIntoYearsAndWeeks(shifts);
            PlanningPeriodModel period1 = new PlanningPeriodModel(1, new DateTime(2019, 12, 30), new DateTime(2020, 4, 12));
            PlanningPeriodModel period2 = new PlanningPeriodModel(2, new DateTime(2020, 4, 13), new DateTime(2020, 8, 2));
            PlanningPeriodModel period3 = new PlanningPeriodModel(3, new DateTime(2020, 8, 3), new DateTime(2020, 11, 22));
            PlanningPeriodModel period4 = new PlanningPeriodModel(4, new DateTime(2020, 11, 23), new DateTime(2021, 3, 14));
            PlanningPeriodModel period5 = new PlanningPeriodModel(5, new DateTime(2021, 3, 15), new DateTime(2021, 7, 4));

            foreach (YearModel year in years) 
            {
                foreach (WeekModel week in year.Weeks)
                {
                    foreach (ShiftModel shift in week.Shifts)
                    {
                        if ((DateTime.Compare(shift.Date, new DateTime(2019, 12, 30)) >= 0) && DateTime.Compare(shift.Date, new DateTime(2020, 4, 12)) <= 0) 
                        {
                            period1.TotalMinutesWorked += shift.ShiftInMinutes - shift.Pause;
                        }
                        else if ((DateTime.Compare(shift.Date, new DateTime(2020, 4, 13)) >= 0) && DateTime.Compare(shift.Date, new DateTime(2020, 8, 2)) <= 0)
                        {
                            period2.TotalMinutesWorked += shift.ShiftInMinutes - shift.Pause;
                        }
                        else if ((DateTime.Compare(shift.Date, new DateTime(2020, 8, 3)) >= 0) && DateTime.Compare(shift.Date, new DateTime(2020, 11, 22)) <= 0)
                        {
                            period3.TotalMinutesWorked += shift.ShiftInMinutes - shift.Pause;
                        }
                        else if ((DateTime.Compare(shift.Date, new DateTime(2020, 11, 23)) >= 0) && DateTime.Compare(shift.Date, new DateTime(2021, 3, 14)) <= 0)
                        {
                            period4.TotalMinutesWorked += shift.ShiftInMinutes - shift.Pause;
                        }
                        else if ((DateTime.Compare(shift.Date, new DateTime(2021, 3, 15)) >= 0) && DateTime.Compare(shift.Date, new DateTime(2021, 7, 4)) <= 0)
                        {
                            period5.TotalMinutesWorked += shift.ShiftInMinutes - shift.Pause;
                        }
                    }
                }
            }
            List<PlanningPeriodModel> planningPeriods = new List<PlanningPeriodModel>();
            planningPeriods.Add(period1);
            planningPeriods.Add(period2);
            planningPeriods.Add(period3);
            planningPeriods.Add(period4);
            planningPeriods.Add(period5);
            CalculateHolidayReduction(planningPeriods);
            return planningPeriods;
        }

        private void CalculateHolidayReduction(List<PlanningPeriodModel> planningPeriods) 
        {
            foreach (PlanningPeriodModel planningPeriod in planningPeriods)
            {
                planningPeriod.CalculateAverageTimeWorked();
            }
        }

        private List<YearModel> DivideShiftsIntoYearsAndWeeks(List<ShiftParentModel> shifts)
        {
            List<YearModel> years = new List<YearModel>();
            int weekCounter = 0;
            int weekIndex = -1;
            int yearCounter = 0;
            int yearIndex = -1;
            foreach (ShiftParentModel shift in shifts)
            {
                if (shift.YearNumber != yearCounter)
                {
                    yearCounter = shift.YearNumber;
                    yearIndex++;
                    years.Add(new YearModel(yearCounter));
                    weekIndex = -1; // every new year the counter should reset 
                }
                if (shift.WeekNumber != weekCounter)
                {
                    weekCounter = shift.WeekNumber;
                    weekIndex++;
                    years[yearIndex].Weeks.Add(new WeekModel(weekCounter));
                }
                years[yearIndex].Weeks[weekIndex].AddShiftToWeek(shift);
            }
            return years;
        }

        private int WeekNumber(DateTime date)
        {
            var dayOffset = DayOfWeek.Thursday - date.DayOfWeek;
            if (dayOffset > 0)
                date = date.AddDays(dayOffset);

            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}