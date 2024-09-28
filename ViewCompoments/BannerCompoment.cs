using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.ViewCompoments
{
    public class BannerCompoment : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public BannerCompoment(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var banner = _db.Banner.ToList();
            return View(banner);
        }
    }
}
