using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Enums;

namespace NHT_Marine_BE.Data.Dtos.Transaction
{
    public class UpdateCouponDto
    {
        [StringLength(50, MinimumLength = 3)]
        public string? Code { get; set; }

        public CouponType? Type { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? Amount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxUsage must be greater than 0")]
        public int? MaxUsage { get; set; }

        public bool? IsActive { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ExpiredAt { get; set; }
    }
}
