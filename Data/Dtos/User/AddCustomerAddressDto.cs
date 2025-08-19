using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.User
{
    public class AddCustomerAddressDto
    {
        [Required]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Ward { get; set; } = string.Empty;

        [Required]
        public string AddressLine { get; set; } = string.Empty;
    }
}
