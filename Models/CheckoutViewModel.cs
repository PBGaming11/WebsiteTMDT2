using WebsiteTMDT.Areas.Admin.Models.EF;

namespace WebsiteTMDT.Models
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
