using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTMDT.Areas.Admin.Models.EF
{
    [Table("tb_Product")]
    public class Product : CommonAbstract
    {
        public Product()
        {
            this.ProductImages = new HashSet<ProductImage>();
            this.OrderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Mời nhập tên sản phẩm")]
        [StringLength(250)]
        public string Title { get; set; }
        public string? Alias { get; set; }
        [Required(ErrorMessage = "Chọn danh mục")]
        public int ProductCategoryId { get; set; }
        public string? ProductCode { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Mời nhập giá sản phẩm")]
        public int Price { get; set; }
        public int? PriceSale { get; set; }
        [Required(ErrorMessage = "Mời nhập số lượng sản phẩm")]
        public int Quality { get; set; } = 1;
        public bool IsHome { get; set; }
        public bool IsHot { get; set; }
        public bool IsSale { get; set; }
        public bool IsFeature { get; set; }
        public string? SeoTitle { get; set; }
        public bool IsActive { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
