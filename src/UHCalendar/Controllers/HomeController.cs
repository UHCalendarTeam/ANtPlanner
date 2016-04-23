using System.Collections.Generic;
using DataLayer;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Mvc;

namespace UHCalendar.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var users = new List<User>();
            using (var context = new CalDavContext())
            {
                context.Users.Add(new User());
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