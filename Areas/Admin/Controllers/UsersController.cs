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

            // Lấy tất cả các role từ database
            var roles = await _roleManager.Roles.ToListAsync();

            // Chuyển đổi từ IdentityRole thành SelectListItem
            var allRoles = roles.Select(role => new SelectListItem
            {
                Value = role.Name,
                Text = role.Name
            }).ToList();

            // Lấy danh sách role hiện tại của user
            var userRoles = await _userManager.GetRolesAsync(user);

            // Tạo model và truyền dữ liệu cho view
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
            user.UserName = model.User.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot update user");
                return View(model);
            }

            // Kiểm tra nếu có mật khẩu mới được nhập
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                // Cập nhật mật khẩu mới cho user nếu có
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passwordChangeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot change password");
                    return View(model);
                }
            }

            // Cập nhật role cho người dùng
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.SelectedRoles ?? new string[] { };

            var rolesToRemove = userRoles.Except(selectedRoles);
            var rolesToAdd = selectedRoles.Except(userRoles);

            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }
    }
}
