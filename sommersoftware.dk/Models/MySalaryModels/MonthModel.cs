using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace sommersoftware.dk.Models.MySalaryModels
{
    public class MonthModel
    {
        public List<ShiftParentModel> Shifts { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; private set; }
        public double TotalMonthSalary { get; set; }
        public string TotalSalaryAfterTax { get; set; }
        public string TotalHoursWorked { get; private set; }
        public string MonthSalaryAsCurrency { get; set; }
        private double TaxPercentage = 0;
        private double TaxDeduction = 0;
        public string EveningSalaryMinutes { get; private set; }
        public string SaturdaySalaryMinutes { get; private set; }
        public string SundaySalaryMinutes { get; private set; }
        public string EveningSalaryTotal { get; private set; }
        public string SaturdaySalaryTotal { get; private set; }
        public string SundaySalaryTotal { get; private set; }
        public string HourSalaryTotal { get; private set; }
        private WageDetailsModel _wages;

        public MonthModel(string name, int yearNumber, double taxPercentage, double taxDeduction, WageDetailsModel wages)
        {
            Shifts = new List<ShiftParentModel>();
            Name = name;
            DisplayName = yearNumber + ", " + Name;
            TaxPercentage = taxPercentage;
            TaxDeduction = taxDeduction;
            _wages = wages;
        }

        public void UpdateTotalMonthSalary()
        {
            int minutesWorked = 0;
            double salaryCount = 0;
            int eveningCount = 0;
            int saturdayCount = 0;
            int sundayCount = 0;
            foreach (ShiftParentModel shift in Shifts)
            {
                minutesWorked += shift.TotalMinutesWorked;
                salaryCount += shift.TotalSalary;
                eveningCount += shift.EveningWageMinutes;
                saturdayCount += shift.SaturdayWageMinutes;
                sundayCount += shift.SundayWageMinutes;
            }
            TotalMonthSalary = salaryCount;
            double salaryAfterTax = TaxPercentage / 100;
            TotalSalaryAfterTax = (((TotalMonthSalary - TaxDeduction) * salaryAfterTax) + TaxDeduction).ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
            MonthSalaryAsCurrency = TotalMonthSalary.ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
            EveningSalaryMinutes = GetPrettyPrintHourMinute(eveningCount);
            SaturdaySalaryMinutes = GetPrettyPrintHourMinute(saturdayCount);
            SundaySalaryMinutes = GetPrettyPrintHourMinute(sundayCount);
            TotalHoursWorked = GetPrettyPrintHourMinute(minutesWorked);
            HourSalaryTotal = (minutesWorked * (_wages.HourWage / 60)).ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
            EveningSalaryTotal = (eveningCount * (_wages.EveningWage / 60)).ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
            SaturdaySalaryTotal = (saturdayCount * (_wages.SaturdayWage / 60)).ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
            SundaySalaryTotal = (sundayCount * (_wages.SundayWage / 60)).ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));

        }
        private string GetStringAsCurrency(int minutes, double wage)
        {
            return String.Format(CultureInfo.CreateSpecificCulture("da-DK"), "kr.  {0:##,###.00}", (minutes * (wage / 60)));
        }

        private string GetPrettyPrintHourMinute(int count)
        {
            int hoursCounter = count / 60;
            int minuteCounter = count % 60;
            return hoursCounter + "h" + minuteCounter + "m";
        }

        public int GetMonthAsNumber()
        {
            if (this.Name.Contains("Jan"))
                return 1;
            else if (this.Name.Contains("Feb"))
                return 2;
            else if (this.Name.Contains("Mar"))
                return 3;
            else if (this.Name.Contains("Apr"))
                return 4;
            else if (this.Name.Contains("May"))
                return 5;
            else if (this.Name.Contains("Jun"))
                return 6;
            else if (this.Name.Contains("Jul"))
                return 7;
            else if (this.Name.Contains("Aug"))
                return 8;
            else if (this.Name.Contains("Sep"))
                return 9;
            else if (this.Name.Contains("Oct"))
                return 10;
            else if (this.Name.Contains("Nov"))
                return 11;
            else if (this.Name.Contains("Dec"))
                return 12;
            else
                return 0;
        }
    }
}