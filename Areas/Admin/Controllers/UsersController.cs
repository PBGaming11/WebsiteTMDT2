using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebsiteTMDT.Models;
using System.Threading.Tasks;
using WebsiteTMDT.Areas.Admin.Models.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            var roles = await _roleManager.Roles.ToListAsync();

            var allRoles = roles
            .Where(role => role.Name != "Admin") // Loại bỏ vai trò Admin
            .Select(role => new SelectListItem
            {
                Value = role.Name,
                Text = role.Name
            }).ToList();

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                User = user,
                Roles = userRoles,
                AllRoles = allRoles,  // Gán danh sách SelectListItem
                SelectedRoles = userRoles.ToArray()  // Nếu bạn muốn hiển thị role đã được chọn
            };

            return View(model);
        }


        // POST: Admin/Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.User.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng (không yêu cầu mật khẩu)
            user.Email = model.User.Email;

            var result = await _userManager.UpdateAsync(user);

            // Kiểm tra lỗi và thêm vào ModelState
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Cập nhật role cho người dùng
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.SelectedRoles ?? new string[] { };

            // Nếu user hiện tại là Admin, không cho phép thêm hoặc xóa vai trò Admin
            if (userRoles.Contains("Admin"))
            {
                selectedRoles = selectedRoles.Except(new[] { "Admin" }).ToArray();
            }

            var rolesToRemove = userRoles.Except(selectedRoles);
            var rolesToAdd = selectedRoles.Except(userRoles);

            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            // Chuyển hướng về trang danh sách người dùng
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }

    }
}
