using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sommersoftware.dk.Models.MySalaryModels
{
    public class WeekModel
    {

        public WeekModel(int weekNumber)
        {
            WeekNumber = weekNumber;
        }
        public List<ShiftParentModel> Shifts { get; private set; } = new List<ShiftParentModel>();
        public int TotalHoursWorked { get; set; } = 0;
        public int WeekNumber { get; set; }

        public void AddShiftToWeek(ShiftParentModel shift)
        {
            Shifts.Add(shift);
            TotalHoursWorked += shift.ShiftInMinutes;
        }
    }
}
