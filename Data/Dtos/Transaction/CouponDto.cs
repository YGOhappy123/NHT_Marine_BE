using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Enums;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public CouponType Type { get; set; }
        public decimal Amount { get; set; }
        public int? MaxUsage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpiredAt { get; set; } = DateTime.Now.AddDays(30); // Default expiration date is 30 days from creation
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public StaffDto? CreatedByStaff { get; set; }
    }
}
