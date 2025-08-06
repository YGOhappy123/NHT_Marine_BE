using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class CreateUpdatePromotionDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int DiscountRate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> Products { get; set; } = [];
    }
}
