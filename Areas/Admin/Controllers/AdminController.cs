using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteTMDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được truy cập
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
