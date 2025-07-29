using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class OrderStatusMapper
    {
        public static OrderStatusDto ToOrderStatusDto(this OrderStatus orderStatus)
        {
            return new OrderStatusDto
            {
                StatusId = orderStatus.StatusId,
                Name = orderStatus.Name,
                Description = orderStatus.Description,
                IsDefaultState = orderStatus.IsDefaultState,
                IsAccounted = orderStatus.IsAccounted,
                IsUnfulfilled = orderStatus.IsUnfulfilled,
            };
        }
    }
}
