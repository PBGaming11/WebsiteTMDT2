namespace WebsiteTMDT.Models
{
    public class OrderTrackingViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public bool ShippingStatus { get; set; }
        public List<OrderDetailViewModel> OrderDetail { get; set; }
    }
    public class OrderDetailViewModel
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
