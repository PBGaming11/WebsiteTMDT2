using System.ComponentModel.DataAnnotations;


namespace WebsiteTMDT.Areas.Admin.Models.EF
{
    public class Banner
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title {get; set;}
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsSingle { get; set; }
    }
}
