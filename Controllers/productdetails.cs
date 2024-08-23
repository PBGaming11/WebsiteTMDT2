using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.Controllers
{
    public class ProductDetails : Controller
    {
        private readonly ILogger<ProductDetails> _logger;
        private readonly ApplicationDbContext _db;

        public ProductDetails(ILogger<ProductDetails> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index(int id ,string alias)
        {
            // Retrieve the product by its Id
            Product sanpham = _db.Products
                                 .Include(s => s.ProductCategory)
                                 .FirstOrDefault(s => s.Id == id);

            if (sanpham == null)
            {
                // Handle the case where the product is not found
                return NotFound();
            }

            var relatedProducts = _db.Products
                                     .Where(p => p.ProductCategoryId == sanpham.ProductCategoryId && p.Id != id)
                                     .OrderBy(p => Guid.NewGuid()) // Randomize
                                     .Take(4)
                                     .ToList();

            ViewBag.RelatedProducts = relatedProducts;
            // Set ViewBag properties for the view
            ViewBag.SelectedCategory = sanpham.ProductCategory;
            ViewBag.SelectedProduct = sanpham;

            return View(sanpham);
        }
    }
}
