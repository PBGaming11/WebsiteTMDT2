using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Models;

namespace WebsiteTMDT.Areas.Admin.Models
{
    public class OrderViewCheckOut
    {
        public Order Order { get; set; }
        public List<OrderDetail> orderDetails { get; set; }
    }
}
