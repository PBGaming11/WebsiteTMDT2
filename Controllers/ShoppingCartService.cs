using WebsiteTMDT.Models;
using YourNamespace.Extensions;

namespace WebsiteTMDT.Controllers
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartItems";

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Tạo phương thức để lấy giỏ hàng từ session
        private List<CartItem> RetrieveCartItemsFromSession()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        // Tạo phương thức để lưu giỏ hàng vào session
        private void SaveCartItemsToSession(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetObjectAsJson(CartSessionKey, cart);
        }

        // Phương thức công khai để lấy các mục trong giỏ hàng
        public List<CartItem> GetCartItems()
        {
            return RetrieveCartItemsFromSession();
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            SaveCartItemsToSession(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartItemsToSession(cart);
            }
        }
    }


}
