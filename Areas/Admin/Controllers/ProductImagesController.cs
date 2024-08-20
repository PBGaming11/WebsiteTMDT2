using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductImagesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductImagesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int id)
        {
            ViewBag.ProductId = id;
            var items = _db.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(items);
        }
        [HttpPost]
        public IActionResult AddImage(int productId, string url)
        {
            _db.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                Image = url,
                IsDefault = false,
            });
            _db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _db.ProductImages.Find(id);
            _db.ProductImages.Remove(item);
            _db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
