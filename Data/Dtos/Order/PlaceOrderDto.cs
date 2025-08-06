using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class PlaceOrderDto
    {
        public string? Note { get; set; }
        public string? Coupon { get; set; }
        public string? RecipientName { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryPhone { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemDto> Items { get; set; } = [];
    }

    public class OrderItemDto
    {
        [Required]
        public int ProductItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
