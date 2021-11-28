using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;


namespace sommersoftware.dk.Models.MySalaryModels
{
    public class ShiftModel : ShiftParentModel
    {

        public ShiftModel(DateTime date, String weekday, int weekNumber, int yearNumber, int startHour,
                          int startMinute, int endHour, int endMinute, WageDetailsModel wages)
        {
            Date = date;
            if (Date.ToString("dd/MM/yyyy").Contains("31/12"))
            {
                // TODO: If the date is the last of the year, hours after 15:00 are added with added holiday wage. 
            }
            Weekday = weekday;
            WeekNumber = weekNumber;
            YearNumber = yearNumber;
            StartHour = startHour;
            StartMinute = startMinute;
            EndHour = endHour;
            EndMinute = endMinute;
            DisplayDateTime = date.ToString("dd/MM/yyyy") + "   " + (startHour < 10 ? "0" + startHour.ToString() : startHour.ToString()) + ":"
                                                                  + (startMinute < 10 ? "0" + startMinute.ToString() : startMinute.ToString()) + " - "
                                                                  + (endHour < 10 ? "0" + endHour.ToString() : endHour.ToString()) + ":"
                                                                  + (endMinute < 10 ? "0" + endMinute.ToString() : endMinute.ToString());
            CalculateShiftInMinutes();
            CalculatePauseTime();
            CalculateHourSalary(wages);
            if (Weekday != "Sunday" && Weekday != "Saturday")
            {
                CalculateEveningSalary(wages);
            }
            if (Weekday == "Saturday")
            {
                CalculateSaturdaySalary(wages);
            }
            if (Weekday == "Sunday")
            {
                CalculateSundaySalary(wages);
            }
            TotalSalary = HourSalary + EveningSalary + SaturdaySalary + SundaySalary;
            SalaryAsCurrency = TotalSalary.ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
            TotalMinutesWorked = ShiftInMinutes - Pause;
        }

        public string Weekday { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int Pause { get; set; }


        private void CalculateShiftInMinutes()
        {
            ShiftInMinutes = (((EndHour * 60) + EndMinute) - ((StartHour * 60) + StartMinute));
        }

        private void CalculatePauseTime()
        {
            if (ShiftInMinutes < 270)
            {
                Pause = 0;
            }
            else if ((ShiftInMinutes >= 270) && (ShiftInMinutes < 420))
            {
                Pause = 30;
            }
            else if ((ShiftInMinutes >= 420) && (ShiftInMinutes < 520))
            {
                Pause = 45;
            }
            else if ((ShiftInMinutes >= 520) && (ShiftInMinutes < 630))
            {
                Pause = 60;
            }
            else if ((ShiftInMinutes >= 630) && (ShiftInMinutes < 720))
            {
                Pause = 75;
            }
            else
            {
                Pause = 90;
            }
        }

        private void CalculateHourSalary(WageDetailsModel userSalaryDetails)
        {
            double minuteSalary = userSalaryDetails.HourWage / 60.0;
            HourSalary = ((ShiftInMinutes - Pause) * minuteSalary);
        }

        private void CalculateEveningSalary(WageDetailsModel userSalaryDetails)
        {
            double minuteSallery = userSalaryDetails.EveningWage / 60.0;
            int countMinutes = 0;
            if (((String.Compare(Weekday, "Sunday")) != 0) && ((String.Compare(Weekday, "Saturday")) != 0))
            {
                for (int j = EndHour; j > 18; j--)
                {
                    countMinutes += 60;
                }
                EveningWageMinutes = countMinutes + EndMinute - Pause;
                EveningSalary = ((countMinutes + EndMinute) - Pause) * minuteSallery;
            }
        }

        private void CalculateSaturdaySalary(WageDetailsModel userSalaryDetails)
        {
            double minuteSallery = userSalaryDetails.SaturdayWage / 60.0;
            int countMinutes = 0;
            if ((String.Compare(Weekday, "Saturday")) == 0)
            {
                if (EndHour < 15 || (EndHour == 15 && EndMinute == 00))
                {
                    SaturdayWageMinutes = 0;
                    SaturdaySalary = 0;
                }
                else
                {
                    for (int j = EndHour; j > 15; j--)
                    {
                        countMinutes += 60;
                    }
                    SaturdaySalary = (countMinutes + EndMinute - Pause) * minuteSallery;
                    SaturdayWageMinutes = countMinutes + EndMinute - Pause; 
                }
            }
        }

        private void CalculateSundaySalary(WageDetailsModel userSalaryDetails)
        {
            double minuteSallery = userSalaryDetails.SundayWage / 60;
            if ((String.Compare(Weekday, "Sunday")) == 0)
            {
                SundayWageMinutes = ShiftInMinutes - Pause;
                SundaySalary = (ShiftInMinutes - Pause) * minuteSallery;
            }
        }
    }
}
