using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteTMDT.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")] // Chỉ Seller mới được truy cập
    public class SellerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
