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

            var viewModel = new CheckoutViewModel
            {
                Order = new Order(), // Tạo một đối tượng Order mới hoặc lấy từ database
                CartItems = cart ?? new List<CartItem>()
            };

            ViewBag.CartItemCount = viewModel.CartItems.Count;
            ViewBag.TotalAmount = viewModel.CartItems.Sum(c => c.Price * c.Quantity);

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

            if (ModelState.IsValid)
            {
                // Nếu ModelState không hợp lệ, quay lại trang thanh toán với dữ liệu giỏ hàng
                model.CartItems = cart;
                return View("Index", model);
            }

            var order = model.Order;

            // Tạo đơn hàng
            order.CreateDate = DateTime.Now;
            order.ModifierDate = DateTime.Now;
            order.Code = GenerateOrderCode();
            order.ShippingStatus = false;
            order.TotalAmount = cart.Sum(c => c.Price * c.Quantity);
            order.Quality = cart.Sum(c => c.Quantity);
            order.OrderDetails = cart.Select(c => new OrderDetail
            {
                ProductId = c.ProductId,
                Quality = c.Quantity,
                Price = c.Price
            }).ToList();

            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["OrderCode"] = order.Code;
            // Xóa giỏ hàng sau khi đặt hàng thành công
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderConfirmation");
        }


        // GET: /Checkout/OrderConfirmation
        public IActionResult OrderConfirmation(string orderCode)
        {
            ViewBag.OrderCode = orderCode;
            return View();
        }
    }
}
