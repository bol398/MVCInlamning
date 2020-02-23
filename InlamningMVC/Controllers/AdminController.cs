using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InlamningMVC.Data;
using InlamningMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace InlamningMVC.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public AdminController (ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Dictionary<string, string> adminPages;

        private bool IsAdmin()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null && _userManager.IsInRoleAsync(user, "Admin").Result)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IActionResult Index()
        {
            
            if (IsAdmin())
            {
                adminPages = new Dictionary<string, string>();
                adminPages.Add("Users", "AdminUsers");
                adminPages.Add("Test", "AdminTest");

                ViewData["adminPages"] = adminPages;
                return View();
            } else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult AdminUsers()
        {
            if (IsAdmin())
            {
                ViewData["usersOnPage"] = _userManager.GetUsersInRoleAsync("Admin");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult AdminTest()
        {
            if (IsAdmin())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult UpdateAdminView(string selectedPage, int currentPage = 1, int usersPerPage = 10)
        {
            if (IsAdmin())
            {
                
                if (selectedPage == "AdminUsers") {
                    if (currentPage >= 1) {
                        ViewData["currentPage"] = currentPage;
                    } else
                    {
                        ViewData["currentPage"] = 1;
                    }
                    if (usersPerPage >= 1)
                    {
                        ViewData["usersPerPage"] = usersPerPage;
                    } else
                    {
                        ViewData["usersPerPage"] = 10;
                    }

                }

                return PartialView(selectedPage);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        public async Task<ActionResult> DeleteUser(string userId)
        {
            if (IsAdmin())
            {
                
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.DeleteAsync(user);
                
                return PartialView("AdminUsers");
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> UserDeletionConfirm(string userId)
        {
            if (IsAdmin())
            {
                ViewData["userIdToDelete"] = userId;
                var user = await _userManager.FindByIdAsync(userId);
                ViewData["userNameToDelete"] = user.FirstName + " " + user.LastName;
                return PartialView();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        public async Task<ActionResult> GenerateUsers(int amount)
        {
            if (IsAdmin())
            {
                var id = 0;
                var users = _userManager.Users.Where(user => user.UserName.Contains("GeneratedTest@"));
                if ( users.Count() != 0) {
                    var split = users.OrderByDescending(user => user.UserName).FirstOrDefault().UserName.Split("@");
                    id = int.Parse(split[1].Split("T")[0]) + 1;
                } 

                for (int i = 0; i < amount; i++)
                {
                    while (_userManager.FindByNameAsync("GeneratedTest@" + id + "Test.com").Result != null)
                    {
                        id++;
                    }

                    var user = new ApplicationUser
                    {
                        UserName = "GeneratedTest@" + id + "Test.com",
                        Email = "GeneratedTest@" + id + "Test.com",
                        FirstName = "FirstName" + id,
                        LastName = "LastName" + id
                    };
                    id++;
                    
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, "User");
                }
                return PartialView("AdminUsers");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<ActionResult> ChangeRole(string userEmail, string role)
        {
            if (IsAdmin())
            {
                var user = await _userManager.FindByNameAsync(userEmail);
                if (role == "Admin")
                {
                    await _userManager.RemoveFromRoleAsync(user,"User");
                }
                if (role == "User")
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                await _userManager.AddToRoleAsync(user, role);
               
                
                return PartialView("AdminUsers");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}