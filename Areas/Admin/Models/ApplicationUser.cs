using Microsoft.AspNetCore.Identity;
namespace WebsiteTMDT.Areas.Admin.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? profileImage { get; set; }
    }
}
