using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<(List<Order>, int)> GetAllOrders(BaseQueryObject queryObject);
        Task<(List<Order>, int)> GetCustomerOrders(BaseQueryObject queryObject, int customerId);
        Task<Order?> GetOrderById(int orderId);
        Task<OrderStatus?> GetDefaultOrderStatus();
        Task<List<StatusTransition>> GetStatusTransitions(int orderStatusId);
        Task<List<OrderStatusUpdateLog>> GetStatusUpdateLogs(int orderId);
        Task AddOrder(Order order);
        Task UpdateOrder(Order order);
        Task ProcessOrderInventory(AcceptOrderDto acceptOrderDto);
        Task AddStatusUpdateLog(OrderStatusUpdateLog log);
        Task<List<Order>> GetAllOrdersInTimeRange(DateTime startTime, DateTime endTime);
    }
}
