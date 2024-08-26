using Microsoft.AspNetCore.Mvc;
using WebsiteTMDT.Data;
using WebsiteTMDT.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList.Extensions;

namespace WebsiteTMDT.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ApplicationDbContext _db;

        public ShoppingCartController(IShoppingCartService shoppingCartService, ApplicationDbContext db)
        {
            _shoppingCartService = shoppingCartService;
            _db = db;
        }
        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItemDto cartItemDto)
        {
            if (cartItemDto == null || cartItemDto.Quantity <= 0)
            {
                return BadRequest("Invalid item.");
            }

            var product = _db.Products.Find(cartItemDto.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            var price = product.PriceSale.HasValue ? product.PriceSale.Value : product.Price;

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Title,
                Price = price,
                Quantity = cartItemDto.Quantity,
                ImageUrl = product.Image // Lấy URL của ảnh sản phẩm
            };

            _shoppingCartService.AddToCart(cartItem);

            // Tính tổng tiền giỏ hàng mới
            var cartItems = _shoppingCartService.GetCartItems();
            var newTotal = cartItems.Sum(item => item.Price * item.Quantity);

            return Json(new { success = true, newTotal = newTotal });
        }


        [HttpGet]
        public IActionResult Cart(int? page)
        {
            int pageSize = 10; // Số lượng mục trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại

            var items = _shoppingCartService.GetCartItems(); // Lấy danh sách sản phẩm trong giỏ hàng

            var pagedItems = items.ToPagedList(pageNumber, pageSize); // Phân trang danh sách sản phẩm
            var cartTotal = items.Sum(item => item.Price * item.Quantity);
            ViewBag.CartPrice = cartTotal;

            return View(pagedItems); // Trả về view với danh sách phân trang
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _shoppingCartService.RemoveFromCart(productId);

            var cartItems = _shoppingCartService.GetCartItems();
            var newTotal = cartItems.Sum(item => item.Price * item.Quantity);

            return Json(new { success = true, newTotal = newTotal });
        }
        [HttpPost]
        public IActionResult UpdateCartItem([FromBody] CartItemDto cartItemDto)
        {
            if (cartItemDto == null || cartItemDto.Quantity <= 0)
            {
                return BadRequest("Invalid item.");
            }

            var product = _db.Products.Find(cartItemDto.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            _shoppingCartService.UpdateCartItem(cartItemDto.ProductId, cartItemDto.Quantity);

            // Calculate the new total
            var cartItems = _shoppingCartService.GetCartItems();
            var newTotal = cartItems.Sum(item => item.Price * item.Quantity);

            return Json(new { success = true, newTotal = newTotal });
        }
        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var cartItems = _shoppingCartService.GetCartItems();
            var count = cartItems.Sum(item => item.Quantity);

            return Json(count);
        }
        [HttpGet]
        public IActionResult GetCartTotal()
        {
            var cartItems = _shoppingCartService.GetCartItems();
            var total = cartItems.Sum(item => item.Price * item.Quantity);

            return Json(new { total = total });
        }



        public IActionResult Checkout(int? page)
        {
            int pageSize = 10; // Số lượng mục trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại

            var items = _shoppingCartService.GetCartItems(); // Lấy danh sách sản phẩm trong giỏ hàng

            var pagedItems = items.ToPagedList(pageNumber, pageSize); // Phân trang danh sách sản phẩm
            var cartTotal = items.Sum(item => item.Price * item.Quantity);
            ViewBag.CartPrice = cartTotal;

            return View(pagedItems); // Trả về view với danh sách phân trang
        }

    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
