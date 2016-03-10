using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CalDAV.Models;

namespace UHCalendar.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var users = new List<User>();
            using (var context = new CalDavContext())
            {
                context.Users.Add(new User() {FirstName = "Adriano", LastName = "Flechilla"});
                context.SaveChanges();

                users.AddRange(context.Users);
            }
            ViewData["users"] = users;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
