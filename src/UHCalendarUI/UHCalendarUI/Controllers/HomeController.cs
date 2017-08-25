using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_Core_1_0.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
           
            ViewData["SubTitle"] = "Welcome " + User.Identity.Name;
            ViewData["Message"] =  "This is your Home Page";

            return View();
        }

        public IActionResult Minor()
        {

            ViewData["SubTitle"] = "Simple example of second view";
            ViewData["Message"] = "Data are passing to view by ViewData from controller";

            return View();
        }
    }
}