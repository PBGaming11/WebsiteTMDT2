using Microsoft.AspNetCore.Mvc;

namespace WebsiteTMDT.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
