using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.Controllers
{
    public class Product_detailsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public Product_detailsController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Index(int id)
        {
           /* Product sanpham = _db.Products.Include(s => s.danhmuc).FirstOrDefault(s => s.IdSanPham == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            // Set ViewBag properties for the view
            ViewBag.SelectedCategory = sanpham.ProductCategory;
            ViewBag.SelectedProduct = sanpham;

            return View(sanpham);*/
            return View();
        }
    }
}
