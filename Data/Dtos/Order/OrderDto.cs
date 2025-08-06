using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? OrderStatusId { get; set; }
        public int? CouponId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? RecipientName { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryPhone { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public CustomerDto? Customer { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public Coupon? Coupon { get; set; }
        public List<OrderItemDataDto> Items { get; set; } = [];
        public List<StatusTransitionDataDto> Transitions { get; set; } = [];
        public List<StatusUpdateLogDataDto> UpdateLogs { get; set; } = [];
    }

    public class OrderItemDataDto
    {
        public int? OrderId { get; set; }
        public int? ProductItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DetailedProductItemDto? ProductItem { get; set; }
    }

    public class StatusTransitionDataDto
    {
        public int TransitionId { get; set; }
        public int? ToStatusId { get; set; }
        public string TransitionLabel { get; set; } = string.Empty;
        public OrderStatus? ToStatus { get; set; }
    }

    public class StatusUpdateLogDataDto
    {
        public int LogId { get; set; }
        public int? OrderId { get; set; }
        public int? StatusId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public StaffDto? UpdatedByStaff { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
