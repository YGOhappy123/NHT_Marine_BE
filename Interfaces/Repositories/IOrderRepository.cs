using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<(List<Order>, int)> GetAllOrders(BaseQueryObject queryObject);
        Task<OrderStatus?> GetDefaultOrderStatus();
        Task<List<StatusTransition>> GetStatusTransitions(int orderStatusId);
        Task<List<OrderStatusUpdateLog>> GetStatusUpdateLogs(int orderId);
        Task AddOrder(Order order);
    }
}
