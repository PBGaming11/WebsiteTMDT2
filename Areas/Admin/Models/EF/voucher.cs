using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTMDT.Areas.Admin.Models.EF
{
    [Table("tb_Voucher")]  // Tùy chọn: xác định bảng trong database
    public class voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Tạo khóa tự động (auto-increment)
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống mã voucher")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm giá")]
        [Range(0, 100, ErrorMessage = "Giá trị giảm phải từ 0 đến 100")]
        public decimal DiscountValue { get; set; }  // Giảm giá dưới dạng % hoặc số tiền cụ thể

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        [CompareDates(nameof(StartDate), ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public bool IsActive { get; set; }
    }

    // Custom Validation Attribute để so sánh ngày
    public class CompareDatesAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;
        public CompareDatesAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDate = (DateTime)validationContext.ObjectType
                .GetProperty(_startDatePropertyName)
                .GetValue(validationContext.ObjectInstance);

            if (value is DateTime endDate && endDate < startDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
