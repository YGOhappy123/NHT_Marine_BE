using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Product
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiscountRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public Staff? CreatedByStaff { get; set; }
        public List<ProductPromotion> Products { get; set; } = [];
    }
}
