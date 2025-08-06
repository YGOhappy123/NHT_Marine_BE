using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class VerifyCouponDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
