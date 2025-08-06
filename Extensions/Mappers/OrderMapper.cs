using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderStatusId = order.OrderStatusId,
                CouponId = order.CouponId,
                TotalAmount = order.TotalAmount,
                RecipientName = order.RecipientName,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryPhone = order.DeliveryPhone,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                Customer = order.Customer?.ToCustomerDto(),
                OrderStatus = order.OrderStatus,
                Coupon = order.Coupon,
                Items = order
                    .Items.Select(item => new OrderItemDataDto
                    {
                        OrderId = item.OrderId,
                        ProductItemId = item.ProductItemId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                    })
                    .ToList(),
            };
        }
    }
}
