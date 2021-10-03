using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace sommersoftware.dk.Models.MySalaryModels
{
    public class SettingsModel
    {
        [Display(Name = "Keyword for all shifts (must be in the title of events)")]
        [Required(ErrorMessage = "No keyword specified!")]
        public string ShiftKeyword { get; set; }

        [Display(Name = "Calendar link to your Google Calendar")]
        [Required]
        public string CalendarLink { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Date to load events from")]
        public DateTime BeginDate { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Date to load events to")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Hour wage")]
        [Required]
        public double HourWage { get; set; }

        [Display(Name = "Evening wage")]
        [Required]
        public double EveningWage { get; set; }

        [Display(Name = "Saturday wage")]
        [Required]
        public double SaturdayWage { get; set; }

        [Display(Name = "Sunday wage")]
        [Required]
        public double SundayWage { get; set; }

        [Display(Name = "Total tax percentage (Multiply all taxes)")]
        [Required]
        public double TotalTaxPercentage { get; set; }

        [Display(Name = "Total tax deduction for each month (Dansk: Fradrag)")]
        [Required]
        public double TotalTaxDeduction { get; set; }
    }
}
