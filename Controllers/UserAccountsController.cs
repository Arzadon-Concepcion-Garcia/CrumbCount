
using CrumbCount2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrumbCount2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserAccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountsController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Display all registered accounts
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();

            return View(users);
        }

        // Display the Create Staff Account form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create a new Staff account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string fullName,
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "",
                    "All fields are required.");

                return View();
            }

            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(
                user,
                password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }
}
