using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InlamningMVC.Models;
using InlamningMVC.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace InlamningMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context), null, null, null, null);
            /*
            var gifAdmin = await _userManager.FindByIdAsync("177b1b57-4619-4500-88b6-676aebfc650f");

            await _userManager.AddToRoleAsync(gifAdmin, "Admin");
            */
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
  
                var role = new IdentityRole();
                role.Name = "Admin";
                await roleManager.CreateAsync(role);

            }
            if (!roleManager.RoleExistsAsync("User").Result)
            {

                var role = new IdentityRole();
                role.Name = "User";
                await roleManager.CreateAsync(role);

            }
            var adminUser = new ApplicationUser
            {
                Email = "admin@admin.se",
                UserName = "admin@admin.se",
                FirstName = "FirstName",
                LastName = "LastName"
            };
            if (!_userManager.Users.Where(user => user.Email == "admin@admin.se").Any())
            {
                await _userManager.CreateAsync(adminUser, "Test1%");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(adminUser);
                await _userManager.ConfirmEmailAsync(adminUser,emailToken);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult WrongCredentials()
        {
            return RedirectToAction("Login", "Account");
        }

    }
}
