using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sommersoftware.dk.Models.MySalaryModels
{
    public class HolidayShiftModel : ShiftParentModel
    {
        public HolidayShiftModel(DateTime date, bool isHolidayReduction, int reductionMinutes, double hourSalary, string summary)
        {
            Date = date;
            IsHolidayReduction = isHolidayReduction;
            if (reductionMinutes == 210)
                _reductionAsHours = "3h30m";
            else if (reductionMinutes == 270)
                _reductionAsHours = "4h30m";
            else if (reductionMinutes == 330)
                _reductionAsHours = "5h30m";
            else if (reductionMinutes == 450)
                _reductionAsHours = "7h30m";
            DisplayDateTime = date.ToString("dd/MM/yyyy") + " " + "Holiday reduction - " + _reductionAsHours + "  (" + summary + ")";
            ShiftInMinutes = reductionMinutes;
            TotalMinutesWorked = ShiftInMinutes;
            TotalSalary = reductionMinutes * (hourSalary / 60);
            SalaryAsCurrency = TotalSalary.ToString("C2", CultureInfo.CreateSpecificCulture("da-DK"));
        }

        private string _reductionAsHours;
    }
}
