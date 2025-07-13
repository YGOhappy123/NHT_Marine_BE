using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Transaction
{
    public class OrderDelivery
    {
        [Key]
        public int DeliveryId { get; set; }
        public int? OrderId { get; set; }
        public int? DeliveryServiceId { get; set; }
        public string TrackingCode { get; set; } = string.Empty;
        public DateTime EstimatedPickupTime { get; set; }
        public Order? Order { get; set; }
        public DeliveryService? DeliveryService { get; set; }
    }
}
