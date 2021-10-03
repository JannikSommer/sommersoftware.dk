using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace sommersoftware.dk.Models.MySalaryModels
{
    public class WageDetailsModel
    {

        public WageDetailsModel() { }

        public WageDetailsModel(double hourWage, double eveningWage, double saturdayWage, double sundayWage)
        {
            HourWage = hourWage;
            EveningWage = eveningWage;
            SaturdayWage = saturdayWage;
            SundayWage = sundayWage;
        }

        public double HourWage { get; set; }
        public double EveningWage { get; set; }
        public double SaturdayWage { get; set; }
        public double SundayWage { get; set; }
    }
}
