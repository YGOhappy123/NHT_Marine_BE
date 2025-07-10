using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Product
{
    public class RootProduct
    {
        [Key]
        public int RootProductId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public ProductCategory? Category { get; set; }
        public Staff? CreatedByStaff { get; set; }
    }
}
