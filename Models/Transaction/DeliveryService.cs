using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Transaction
{
    public class DeliveryService
    {
        [Key]
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
    }
}
