using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class CreateUpdateDeliveryServiceDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string ContactPhone { get; set; } = string.Empty;
    }
}
