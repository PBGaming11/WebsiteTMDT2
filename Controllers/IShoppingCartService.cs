using WebsiteTMDT.Models;

namespace WebsiteTMDT.Controllers
{
    public interface IShoppingCartService
    {
        void AddToCart(CartItem cartItem);
        void RemoveFromCart(int productId);
        List<CartItem> GetCartItems();
        void UpdateCartItem(int productId, int quantity); // Interface method
    }

}
