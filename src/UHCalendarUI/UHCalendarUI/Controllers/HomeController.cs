using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Models.Identity;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

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

            if (user != null)
            {
                var principal = _principalRepository.FindByPrincipalIdWithAll(user.PrincipalId);

                return View("~/Views/Calendar/Calendar.cshtml", principal);
            }

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