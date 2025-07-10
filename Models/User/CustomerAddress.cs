using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.User
{
    public class CustomerAddress
    {
        [Key]
        public int AddressId { get; set; }
        public int? CustomerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public Customer? Customer { get; set; }
    }
}
