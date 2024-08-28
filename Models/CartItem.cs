namespace WebsiteTMDT.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } // URL của hình ảnh sản phẩm
        public bool Issale { get; set; }
    }

}
