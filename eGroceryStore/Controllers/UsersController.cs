using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eGroceryStore.Data;
using eGroceryStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using eGroceryStore.Areas.Data;

namespace eGroceryStore.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public UsersController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            if (!User.IsInRole("admin"))
            {
                return Unauthorized();
            }

            var users = _db.Users.ToList();
            return View(users);
        }


        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserData()
        {
            var user = await  _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }

    }
}
