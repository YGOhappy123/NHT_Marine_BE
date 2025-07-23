using NHT_Marine_BE.Data.Dtos.Auth;

namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class PromotionDto
    {
        public int PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiscountRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public StaffDto? CreatedByStaff { get; set; }
    }
}
