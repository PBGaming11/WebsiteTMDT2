using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace WebsiteTMDT.Areas.Admin.Models.EF
{
    // EditUserViewModel.cs
    public class EditUserViewModel
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
        public IList<SelectListItem> AllRoles { get; set; }  // Sử dụng SelectListItem cho dropdown
        public string[] SelectedRoles { get; set; }
        public string CurrentRole { get; set; }  // Trường để lưu role hiện tại
        public string NewPassword { get; set; }  // Trường để cập nhật mật khẩu mới
    }

}
