using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteTMDT.Data;
using WebsiteTMDT.Models;
using X.PagedList.Extensions;

namespace WebsiteTMDT.Areas.User.Controllers
{
    [Area("User")]
    [CartTotalFilter]
    [CartItemCountFilter]
    public class Cart : Controller
    {
        private readonly ApplicationDbContext _db;
        public Cart(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? page)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            int pageSize = 5; // Số lượng sản phẩm mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1
            // Sử dụng IPagedList để phân trang
            var pagedCart = cart.ToPagedList(pageNumber, pageSize);
            return View(pagedCart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Lấy sản phẩm từ database
            var product = _db.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return Json(new { success = false });
            }

            // Lấy giá hiện tại của sản phẩm, bao gồm giá giảm nếu có
            var currentPrice = product.PriceSale.HasValue ? product.PriceSale.Value : product.Price;

            // Tạo CartItem
            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Title,
                Price = currentPrice, // Sử dụng giá hiện tại
                Quantity = 1, // Số lượng mặc định
                ImageUrl = product.Image,
                Issale = product.IsSale
            };

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.FirstOrDefault(c => c.ProductId == cartItem.ProductId);

            if (existingItem != null)
            {
                // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng
                existingItem.Quantity++;
            }
            else
            {
                // Nếu không, thêm sản phẩm mới vào giỏ hàng
                cart.Add(cartItem);
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Trả về số lượng sản phẩm hiện tại trong giỏ hàng
            return Json(new { success = true, cartItemCount = cart.Sum(c => c.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart == null || quantity <= 0)
            {
                return Json(new { success = false, message = "Giỏ hàng không hợp lệ hoặc số lượng không đúng." });
            }

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            // Cập nhật số lượng
            cartItem.Quantity = quantity;

            // Cập nhật giá sản phẩm từ database
            var product = _db.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var currentPrice = product.PriceSale.HasValue ? product.PriceSale.Value : product.Price;
                cartItem.Price = currentPrice;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Tính tổng tiền
            var totalAmount = cart.Sum(c => c.Price * c.Quantity);

            return Json(new { success = true, totalAmount = totalAmount });
        }

        [HttpGet]
        public IActionResult GetCartTotalAmount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            int totalAmount = cart?.Sum(c => c.Price * c.Quantity) ?? 0;
            return Json(new { totalAmount = totalAmount });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart == null)
            {
                return Json(new { success = false, message = "Giỏ hàng không tồn tại." });
            }

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            cart.Remove(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Tính tổng tiền sau khi xóa sản phẩm
            var totalAmount = cart.Sum(c => c.Price * c.Quantity);
            var cartItemCount = cart.Sum(c => c.Quantity);

            return Json(new { success = true, totalAmount = totalAmount, cartItemCount = cartItemCount });
        }
        //chức năng mua ngay
        [HttpPost]
        public IActionResult BuyNow(int productId)
        {
            var product = _db.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Sử dụng giá giảm nếu có, nếu không sử dụng giá gốc
            var price = product.PriceSale.HasValue ? product.PriceSale.Value : product.Price;

            // Logic thêm sản phẩm vào giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem == null)
            {
                cart.Add(new CartItem { ProductId = productId, Quantity = 1, Price = price ,ProductName = product.Title, ImageUrl = product.Image, Issale = product.IsSale});
            }
            else
            {
                cartItem.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Chuyển đến trang thanh toán
            return RedirectToAction("Index", "CheckOut");
        }
        [HttpPost]
        public IActionResult ApplyVoucher(string voucherCode)
        {
            // Tìm voucher có mã tương ứng và chưa hết hạn, còn hiệu lực và còn số lượng
            var voucher = _db.vouchers
                .FirstOrDefault(v => v.Code == voucherCode
                                     && v.StartDate <= DateTime.Now
                                     && v.EndDate >= DateTime.Now
                                     && v.Quantity > 0
                                     && v.IsActive);

            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher không hợp lệ hoặc đã hết hạn!" });
            }

            // Lưu giá trị giảm giá vào session
            HttpContext.Session.SetString("VoucherCode", voucher.Code);
            HttpContext.Session.SetInt32("VoucherDiscountValue", (int)voucher.DiscountValue);

            return Json(new { success = true, discountValue = voucher.DiscountValue });
        }

    }
}
