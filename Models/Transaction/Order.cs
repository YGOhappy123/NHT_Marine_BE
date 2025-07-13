using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Transaction
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? OrderStatusId { get; set; }
        public int? CouponId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public string? RecipientName { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryPhone { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Customer? Customer { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public Coupon? Coupon { get; set; }
        public List<OrderItem> Items { get; set; } = [];
    }
}
