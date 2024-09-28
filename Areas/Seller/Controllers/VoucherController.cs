using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;

namespace WebsiteTMDT.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")] // Chỉ Seller mới được truy cập
    public class VoucherController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VoucherController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var vouchers = _db.vouchers.ToList();
            return View(vouchers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(voucher model)
        {
            if (ModelState.IsValid)
            {
                _db.vouchers.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Index", "voucher", new { area = "Seller" });
            }
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var voucher = _db.vouchers.Find(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(voucher voucher)
        {
            if (ModelState.IsValid)
            {
                _db.vouchers.Update(voucher);
                _db.SaveChanges();
                return RedirectToAction("Index", "voucher", new { area = "Seller" });
            }
            return View(voucher);
        }
        public IActionResult Delete(int id)
        {
            var item = _db.vouchers.Find(id);
            if (item != null)
            {
                _db.vouchers.Remove(item);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
