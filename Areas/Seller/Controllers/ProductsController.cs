using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;
using X.PagedList.Extensions;

namespace WebsiteTMDT.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")] // Chỉ Seller mới được truy cập
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext db, ILogger<ProductsController> logger)
        {
            _logger = logger;

            _db = db;
        }

        public IActionResult Index(string searchText, int? page)
        {
            var pageSize = 10;
            var pageIndex = page ?? 1;

            // Start with the base query
            IEnumerable<Product> items = _db.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages) // Giả sử ProductImages là thuộc tính của Product
                .ToList();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(x => x.Alias.Contains(searchText) || x.Title.Contains(searchText));
            }

            // Apply pagination after filtering
            var pagedItems = items.ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageIndex;

            return View(pagedItems);
        }
        public IActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(_db.ProductCategories.ToList(), "Id", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Product model, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ProductCategory = new SelectList(_db.ProductCategories.ToList(), "Id", "Title");
                return View(model);
            }

            if(Images != null && Images.Count > 0)
            {
                for(int i = 0; i <Images.Count; i++)
                {
                    if (i + 1 == rDefault[0])
                    {
                        model.Image = Images[i];
                        model.ProductImages.Add(new ProductImage
                        {
                            ProductId = model.Id,
                            Image = Images[i],
                            IsDefault = true,
                        });
                    }
                    else
                    {
                        model.ProductImages.Add(new ProductImage
                        {
                            ProductId = model.Id,
                            Image = Images[i],
                            IsDefault = false,
                        });
                    }

                }
            }
            model.CreateDate = DateTime.Now;
            model.ModifierDate = DateTime.Now;
            if(string.IsNullOrEmpty(model.SeoTitle))
            {
                model.SeoTitle = model.Title;
            }
            if(string.IsNullOrEmpty(model.Alias))
            {
                model.Alias = WebsiteTMDT.Areas.Admin.Models.Common.Filter.FilterChar(model.Title);

            }
            _db.Products.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var item = _db.Products.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.ProductCategoryList = _db.ProductCategories.ToList();
            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product model)
        {
            // Set the ProductCategory based on the selected ProductCategoryId
            model.ProductCategory = _db.ProductCategories.FirstOrDefault(x => x.Id == model.ProductCategoryId);

            if (!ModelState.IsValid)
            {
                // Update additional fields before saving
                model.ModifierDate = DateTime.Now;
                model.Alias = WebsiteTMDT.Areas.Admin.Models.Common.Filter.FilterChar(model.Title);

                // Save the changes to the database
                _db.Update(model);
                _db.SaveChanges();

                // Redirect to Index if the update is successful
                return RedirectToAction("Index");
            }

            // If the ModelState is invalid, re-populate the ViewBag.ProductCategoryList
            ViewBag.ProductCategoryList = _db.ProductCategories.ToList();

            // Return the view with the model to display validation errors
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _db.Products.Find(id);
            if (item != null)
            {
                _db.Products.Remove(item);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        //chỉnh hiện thị trong tin tức
        [HttpPost]
        public IActionResult IsActive(int id)
        {
            var item = _db.Products.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActive });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult DeleteAll(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var items = id.Split(",");
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = _db.Products.Find(Convert.ToInt32(item));
                        _db.Products.Remove(obj);
                        _db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}
