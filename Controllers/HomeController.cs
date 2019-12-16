using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jira.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Jira.Models;
using Microsoft.AspNetCore.Identity;

namespace Jira.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Member") || !User.Identity.IsAuthenticated) return View();

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            await _userManager.AddToRoleAsync(user, "Member");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}