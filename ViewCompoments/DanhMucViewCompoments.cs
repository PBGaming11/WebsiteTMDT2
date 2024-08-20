
using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Data;

namespace Website.ViewCompoments
{
    public class DanhMucViewCompoments : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public DanhMucViewCompoments(ApplicationDbContext db)
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
