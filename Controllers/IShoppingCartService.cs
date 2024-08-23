using WebsiteTMDT.Models;

namespace WebsiteTMDT.Controllers
{
    public interface IShoppingCartService
    {
        void AddToCart(CartItem item);
        List<CartItem> GetCartItems();
        void RemoveFromCart(int productId);
    }

}
