using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.ViewCompoments
{
    public class DanhMucHome : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public DanhMucHome(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var danhmuc = _db.ProductCategories.ToList();
            return View(danhmuc);
        }
    }
}
