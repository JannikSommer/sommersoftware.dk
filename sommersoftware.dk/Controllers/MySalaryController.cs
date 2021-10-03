using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Calendar.v3.Data;
using System.Globalization;
using sommersoftware.dk.Models.MySalaryModels;

namespace sommersoftware.dk.Controllers
{
    public class MySalaryController : Controller
    {
        private readonly GoogleCalendarAPI GoogleCalendarAPI = new GoogleCalendarAPI();

        public IActionResult Index()
        {
            return View();
        }

        [GoogleScopedAuthorize(CalendarService.ScopeConstants.CalendarEventsReadonly)]
        public async Task<IActionResult> Dashboard([FromQuery] string settingsString, [FromServices] IGoogleAuthProvider auth)
        {
            var cred = await auth.GetCredentialAsync();
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });

            // Deserialize the user settings to object
            SettingsModel settings = JsonConvert.DeserializeObject<SettingsModel>(settingsString);
            ViewData["Settings"] = settings;

            // getting the data by Calendar.Service and the user settings
            List<YearModel> years = await GoogleCalendarAPI.GetYears(settings, service);
            foreach (YearModel year in years)
            {
                foreach (MonthModel month in year.Months.ToList())
                {
                    if (month.Shifts.Count == 0)
                        year.Months.Remove(month);
                    else
                        month.UpdateTotalMonthSalary();
                }
            }
            return View(years);
        }

        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Settings(
        [Bind("ShiftKeyword, CalendarLink, BeginDate, EndDate, HourWage, EveningWage, SaturdayWage, SundayWage, TotalTaxPercentage, TotalTaxDeduction")] SettingsModel settings)
        {
            if (ModelState.IsValid)
            {
                string settingsStr = JsonConvert.SerializeObject(settings);
                return RedirectToAction("Dashboard", "MySalary", new { settingsString = settingsStr});
            }
            return View(settings);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Help()
        {
            return View();
        }
    }
}
