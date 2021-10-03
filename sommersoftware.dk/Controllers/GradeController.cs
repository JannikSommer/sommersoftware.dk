using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sommersoftware.dk.Controllers
{
    public class GradeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
