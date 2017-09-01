using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Models.Identity;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using UHCalendarUI.Models;

namespace ASPNET_Core_1_0.Controllers
{
    public class HomeController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        private IPrincipalRepository _principalRepository;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IPrincipalRepository principalRepository)
        {
            _userManager = userManager;
            _principalRepository = principalRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["SubTitle"] = "Welcome " + User.Identity.Name;
            ViewData["Message"] = "This is your Home Page";

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return this.View();
            }

            var principal = this._principalRepository.FindByPrincipalIdWithAll(user.PrincipalId);
            var principalVM = new PrincipalViewModel()
            {
                CalendarHomeName = principal.CalendarHome.Name,
                Email = principal.PrincipalStringIdentifier,
                Url = principal.PrincipalUrl,
                CalendarCollections = principal.CalendarHome.CalendarCollections
            };
            var first = principal.CalendarHome.CalendarCollections.FirstOrDefault(
                c => c.Name == "DefaultCollection" || c.Name == "PublicEvents");
            var currentCalendars = new List<CalendarCollection>();
            if (first != null)
            {
                currentCalendars.Add(first);
            }

            principalVM.CurrentCallendars = currentCalendars;

            return this.View("~/Views/Calendar/Calendar.cshtml", principalVM);
        }

        public IActionResult Minor()
        {
            ViewData["SubTitle"] = "Simple example of second view";
            ViewData["Message"] = "Data are passing to view by ViewData from controller";

            return View();
        }
    }
}