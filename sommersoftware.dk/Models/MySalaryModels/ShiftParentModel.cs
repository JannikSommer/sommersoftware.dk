using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace sommersoftware.dk.Models.MySalaryModels
{
    public class ShiftParentModel : IComparable<ShiftParentModel>
    {
        public DateTime Date { get; set; }
        public int WeekNumber { get; set; }
        public int YearNumber { get; set; }
        public int ShiftInMinutes { get; set; }
        public double HourSalary { get; set; }
        public double EveningSalary { get; set; }
        public double SaturdaySalary { get; set; }
        public double SundaySalary { get; set; }
        public double TotalSalary { get; set; }
        public string SalaryAsCurrency { get; set; }
        public string DisplayDateTime { get; protected set; }
        public bool IsHolidayReduction { get; protected set; }
        public int TotalMinutesWorked { get; protected set; }
        public int EveningWageMinutes { get; protected set; }
        public int SaturdayWageMinutes { get; protected set; }
        public int SundayWageMinutes { get; protected set; }


        public int CompareTo(ShiftParentModel other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return this.Date.CompareTo(other.Date);
            }
        }
    }
}
