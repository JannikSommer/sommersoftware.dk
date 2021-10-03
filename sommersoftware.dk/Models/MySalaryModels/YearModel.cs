using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace sommersoftware.dk.Models.MySalaryModels
{
    public class YearModel
    {
        public int YearNumber { get; set; }
        public List<MonthModel> Months { get; set; }
        public List<WeekModel> Weeks { get; set; }

        public YearModel(int yearNumber, double taxPercentage, double taxDeduction, WageDetailsModel wages)
        {
            YearNumber = yearNumber;
            Months = new List<MonthModel>();
            Months.Add(new MonthModel("January", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("February", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("March", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("April", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("May", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("June", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("July", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("August", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("September", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("October", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("November", YearNumber, taxPercentage, taxDeduction, wages));
            Months.Add(new MonthModel("December", YearNumber, taxPercentage, taxDeduction, wages));
        }

        public YearModel(int yearNumber)
        {
            YearNumber = yearNumber;
            Weeks = new List<WeekModel>();
        }
    }
}
