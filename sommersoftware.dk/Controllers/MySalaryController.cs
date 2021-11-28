using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.AspNetCore3;
using sommersoftware.dk.Models.MySalaryModels;
using sommersoftware.dk.APIs.GoogleCalendarAPI;

namespace sommersoftware.dk.Controllers
{
    public class MySalaryController : Controller
    {
        private readonly GoogleCalendarAPI GoogleCalendarAPI = new GoogleCalendarAPI();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        // TODO: Change from query to header
        [GoogleScopedAuthorize(CalendarService.ScopeConstants.CalendarEventsReadonly)]
        public async Task<IActionResult> Dashboard([FromQuery] string settingsString, [FromServices] IGoogleAuthProvider auth)
        {
            // Get credentials
            var cred = await auth.GetCredentialAsync();
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });

            // Deserialize the user settings to object
            SettingsModel settings = JsonConvert.DeserializeObject<SettingsModel>(settingsString);
            ViewData["Settings"] = settings;

            // Getting the data by Calendar.Service and the user settings
            List<YearModel> years = await GoogleCalendarAPI.GatherCalendarData(settings, service);

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
                // TODO: When canging from query to header, make sure this is updated. 
                return RedirectToAction("Dashboard", "MySalary", new { settingsString = settingsStr});
            }
            return View(settings);
        }
    }
}
