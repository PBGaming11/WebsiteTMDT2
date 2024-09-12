using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Data;
using X.PagedList.Extensions;

namespace WebsiteTMDT.Areas.User.Controllers
{
    [Area("User")]
    public class News : Controller
    {
        private readonly ApplicationDbContext _context;

        public News(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            var pageSize = 10;
            var pageIndex = page ?? 1;

            var newsList = _context.News
                                   .Where(n => n.IsActive)
                                   .OrderByDescending(n => n.CreateDate)
                                   .ToList();

            var pagedItems = newsList.ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageIndex;

            return View(pagedItems);
        }
        public IActionResult Details(string alias)
        {
            var newsItem = _context.News.FirstOrDefault(n => n.Alias == alias && n.IsActive);
            if (newsItem == null)
            {
                return NotFound();
            }
            return View(newsItem);
        }
    }
}
