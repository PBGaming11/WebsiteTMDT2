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

        // Retrieve the cart items from the session
        private List<CartItem> RetrieveCartItemsFromSession()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        // Save the cart items to the session
        private void SaveCartItemsToSession(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetObjectAsJson(CartSessionKey, cart);
        }

        // Public method to get cart items
        public List<CartItem> GetCartItems()
        {
            return RetrieveCartItemsFromSession();
        }

        // Add a new item or update quantity if the item exists in the cart
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

        // Remove an item from the cart
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

        // Update the quantity of a specific item in the cart
        public void UpdateCartItem(int productId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCartItemsToSession(cart);
            }
        }
    }
}
