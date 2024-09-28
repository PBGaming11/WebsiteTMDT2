using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;
using WebsiteTMDT.Models;

namespace WebsiteTMDT.Areas.User.Controllers
{
    [Area("User")]
    public class CheckOutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckOutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            // Lấy giá trị giảm giá từ session (nếu có)
            var discountValue = HttpContext.Session.GetInt32("VoucherDiscountValue") ?? 0;

            // Gán discount value cho từng sản phẩm trong giỏ hàng
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    // Giả sử discountValue là phần trăm
                    item.DiscountValue = item.Price * (discountValue / 100m);
                }
            }

            var viewModel = new CheckoutViewModel
            {
                Order = new Order(), // Tạo một đối tượng Order mới hoặc lấy từ database
                CartItems = cart ?? new List<CartItem>()
            };

            ViewBag.CartItemCount = viewModel.CartItems.Count;
            ViewBag.TotalAmount = viewModel.CartItems.Sum(c => (c.Price - c.DiscountValue) * c.Quantity); // Tính tổng tiền sau giảm giá

            return View(viewModel);
        }


        private string GenerateOrderCode()
        {
            // Mã đơn hàng bắt đầu với "DH" + thời gian hiện tại + một số ngẫu nhiên từ 1000 đến 9999
            return "DH" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }
        [HttpPost]
        public IActionResult ProcessOrder(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            var voucherCode = HttpContext.Session.GetString("VoucherCode");
            // Lấy giá trị giảm giá từ session (nếu có)
            var discountValue = HttpContext.Session.GetInt32("VoucherDiscountValue") ?? 0;

            // Tính tổng tiền trước giảm giá
            var totalAmount = cart.Sum(c => c.Price * c.Quantity);

            // Tính tiền giảm giá theo phần trăm
            var discountAmount = totalAmount * discountValue / 100;

            // Tính tổng tiền sau giảm giá
            var totalAfterDiscount = totalAmount - discountAmount;

            if (ModelState.IsValid)
            {
                model.CartItems = cart;
                return View("Index", model);
            }

            var order = model.Order;

            // Tạo đơn hàng
            order.CreateDate = DateTime.Now;
            order.ModifierDate = DateTime.Now;
            order.Code = GenerateOrderCode();
            order.ShippingStatus = false;
            order.TotalAmount = totalAfterDiscount; // Tổng tiền sau khi giảm
            order.Quality = cart.Sum(c => c.Quantity);
            // Lưu mã voucher và giá trị giảm vào đơn hàng (nếu có voucher)
            order.VoucherCode = voucherCode; // Mã voucher đã sử dụng
            order.DiscountAmount = discountAmount; // Số tiền giảm giá
            order.OrderDetails = cart.Select(c => new OrderDetail
            {
                ProductId = c.ProductId,
                Quality = c.Quantity,
                Price = c.Price
            }).ToList();

            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["OrderCode"] = order.Code;

            // Xóa giỏ hàng và voucher sau khi đặt hàng thành công
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("VoucherCode");
            HttpContext.Session.Remove("VoucherDiscountValue");

            return RedirectToAction("OrderConfirmation", "CheckOut", new { area = "User" });
        }


        public IActionResult OrderConfirmation(string orderCode)
        {
            ViewBag.OrderCode = orderCode;
            return View();
        }
    }
}
