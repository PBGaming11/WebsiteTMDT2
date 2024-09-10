using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebsiteTMDT.Models;
using System.Threading.Tasks;
using WebsiteTMDT.Areas.Admin.Models.EF;

namespace WebsiteTMDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: Admin/Users/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new EditUserViewModel
            {
                User = user,
                Roles = userRoles,
                AllRoles = allRoles
            };

            return View(model);
        }

        // POST: Admin/Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.User.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.User.Email;
            user.UserName = model.User.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot update user");
                return View(model);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.SelectedRoles ?? new string[] { };

            var rolesToRemove = userRoles.Except(selectedRoles);
            var rolesToAdd = selectedRoles.Except(userRoles);

            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            // Redirect về action Index trong controller Users trong area Admin 
            return RedirectToAction("Index", "Users", new { area = "Admin" });

        }

    }
}
