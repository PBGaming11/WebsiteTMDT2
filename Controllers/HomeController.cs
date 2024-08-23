using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;
using WebsiteTMDT.Models;
using System.Linq;

namespace WebsiteTMDT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IShoppingCartService _shoppingCartService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _db = db;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> sanpham = _db.Products.ToList();

            // Lấy số lượng sản phẩm trong giỏ hàng
            var cartItems = _shoppingCartService.GetCartItems();
            ViewBag.CartItemCount = cartItems.Sum(item => item.Quantity); // Tổng số lượng sản phẩm

            return View(sanpham);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
