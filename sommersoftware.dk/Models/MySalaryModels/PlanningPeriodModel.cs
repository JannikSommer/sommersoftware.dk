using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace sommersoftware.dk.Models.MySalaryModels
{
    public class PlanningPeriodModel
    {
        public PlanningPeriodModel(int id, DateTime periodStart, DateTime periodEnd)
        {
            ID = id;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            
        }
        public int ID { get; private set; }
        public int TotalMinutesWorked { get; set; }
        public int AverageMinutesWorked { get; set; }
        public int ReductionAmount { get; private set; }
        public DateTime PeriodStart { get; private set; }
        public DateTime PeriodEnd { get; private set; }


        public void CalculateAverageTimeWorked()
        {
            AverageMinutesWorked = TotalMinutesWorked / 16;
            if (AverageMinutesWorked > 600 && AverageMinutesWorked <= 1200)
            {
                ReductionAmount = 210;
            }
            else if (AverageMinutesWorked < 1200 && AverageMinutesWorked <= 1500)
            {
                ReductionAmount = 270;
            }
            else if (AverageMinutesWorked < 1500 && AverageMinutesWorked <= 1800 )
            {
                ReductionAmount = 330;
            }
            else if (AverageMinutesWorked < 1800)
            {
                ReductionAmount = 450;
            }
        }
    }
}
