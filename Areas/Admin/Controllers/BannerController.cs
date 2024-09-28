using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được truy cập
    public class BannerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BannerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var items = _db.Banner.ToList();
            return View(items);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Banner model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _db.Banner.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            var item = _db.Banner.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Banner model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _db.Update(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Banner", new { area = "Admin" });
        }
        [HttpPost]
        public IActionResult IsActive(int id)
        {
            var item = _db.Banner.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                // Nếu isActive là true, thì đặt isSingle thành false
                if (item.IsActive)
                {
                    item.IsSingle = false;
                }
                _db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActive, isSingle = item.IsSingle });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult IsSingle(int id)
        {
            var item = _db.Banner.Find(id);
            if (item != null)
            {
                // Đổi trạng thái của item hiện tại
                item.IsSingle = !item.IsSingle;

                // Biến để lưu item cần đặt IsSingle = false (nếu có)
                Banner itemToUnset = null;

                if (item.IsSingle)
                {
                    // Đặt IsActive thành false vì chỉ có một trong hai có thể true
                    item.IsActive = false;

                    // Kiểm tra số lượng banner hiện đang có IsSingle = true
                    var singleItems = _db.Banner.Where(b => b.IsSingle).OrderBy(b => b.Id).ToList();
                    if (singleItems.Count >= 2)
                    {
                        // Đặt banner IsSingle cũ nhất thành false
                        itemToUnset = singleItems.First();
                        itemToUnset.IsSingle = false;
                        _db.Entry(itemToUnset).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }

                _db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();

                // Kiểm tra nếu itemToUnset khác null để trả về unsetItemId
                return Json(new { success = true, isActive = item.IsActive, isSingle = item.IsSingle, unsetItemId = itemToUnset?.Id });
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _db.Banner.Find(id);
            if (item != null)
            {
                //var DeleteItem = _db.Categories.Attach(item);
                _db.Banner.Remove(item);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });

        }
    }
}
