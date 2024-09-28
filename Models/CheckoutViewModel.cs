using WebsiteTMDT.Areas.Admin.Models.EF;

namespace WebsiteTMDT.Models
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<CartItem> CartItems { get; set; }
        public string VoucherCode { get; set; } // Mã voucher
        public bool UseVoucher { get; set; } // Kiểm tra có sử dụng voucher không
    }
}
