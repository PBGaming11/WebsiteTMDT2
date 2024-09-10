using Microsoft.AspNetCore.Identity;

namespace WebsiteTMDT.Areas.Admin.Models.EF
{
    public class EditUserViewModel
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
        public IList<IdentityRole> AllRoles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}
