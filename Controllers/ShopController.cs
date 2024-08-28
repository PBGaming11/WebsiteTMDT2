using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.Controllers
{
    [CartTotalFilter]
    [CartItemCountFilter]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ShopController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string categoryAlias, double? minPrice, double? maxPrice, string sortOrder, string searchQuery, int pageNumber = 1, int pageSize = 6)
        {
            var products = _db.Products.AsQueryable();
            ProductCategory selectedCategory = null;

            // Tìm danh mục dựa trên Alias
            if (!string.IsNullOrEmpty(categoryAlias))
            {
                selectedCategory = _db.ProductCategories.FirstOrDefault(c => c.Alias == categoryAlias);
                if (selectedCategory != null)
                {
                    products = products.Where(p => p.ProductCategoryId == selectedCategory.Id);
                }
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Filter by search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Title.Contains(searchQuery));
            }

            // Sort products based on the selected order
            products = sortOrder switch
            {
                "asc" => products.OrderBy(p => p.Price),
                "desc" => products.OrderByDescending(p => p.Price),
                _ => products
            };

            // Calculate total pages
            int totalItems = products.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Get the products for the current page
            var paginatedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Pass data to the view
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CategoryAlias = categoryAlias;
            ViewBag.SelectedCategory = selectedCategory;
            ViewBag.SearchQuery = searchQuery;

            return View(paginatedProducts);
        }

    }
}
