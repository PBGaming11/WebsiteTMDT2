using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteTMDT.Areas.Admin.Models;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;
using X.PagedList.Extensions;

namespace WebsiteTMDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string searchText, int? page)
        {
            var pageSize = 10;
            var pageIndex = page ?? 1;

            // Start with the base query
            IEnumerable<Order> items = _db.Orders.OrderByDescending(x => x.CreateDate);

            // Apply search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(x => x.Code.Contains(searchText) || x.CustomerName.Contains(searchText));
            }

            // Apply pagination after filtering
            var pagedItems = items.ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageIndex;

            return View(pagedItems);
        }
        public IActionResult View(int id)
        {
            // Lấy thông tin đơn hàng dựa trên id
            var order = _db.Orders.Find(id);
            if (order == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy đơn hàng
            }

            // Lấy danh sách chi tiết đơn hàng dựa trên OrderId
            var orderDetails = _db.OrderDetails
                      .Include(od => od.Product)  // Bao gồm Product trong truy vấn
                      .Where(od => od.OrderId == id)
                      .ToList();

            // Tạo một instance của ViewModel và gán giá trị
            var viewModel = new OrderViewCheckOut
            {
                Order = order,
                orderDetails = order.OrderDetails.ToList()
            };

            // Trả về view với ViewModel
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult UpdateShippingStatus(int orderId, bool shippingStatus)
        {
            // Kiểm tra giá trị nhận được
            Console.WriteLine($"OrderId: {orderId}, ShippingStatus: {shippingStatus}");

            var order = _db.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound(); // Nếu không tìm thấy đơn hàng
            }

            // Cập nhật trạng thái giao hàng
            order.ShippingStatus = shippingStatus;

            try
            {
                _db.Orders.Update(order); // Đánh dấu order là đã được cập nhật
                _db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Có lỗi xảy ra khi cập nhật trạng thái giao hàng.");
            }

            return RedirectToAction("Index");
        }

    }
}
