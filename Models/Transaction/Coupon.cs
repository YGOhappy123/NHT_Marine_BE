using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Transaction
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public CouponType Type { get; set; } = CouponType.Fixed;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public int? MaxUsage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpiredAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public Staff? CreatedByStaff { get; set; }
    }
}
