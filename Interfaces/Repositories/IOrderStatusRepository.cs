using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IOrderStatusRepository
    {
        Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<OrderStatus>, int)> GetAllOrderStatuses(BaseQueryObject queryObject);
        Task<OrderStatus?> GetOrderStatusById(int statusId);
        Task<OrderStatus?> GetOrderStatusByName(string statusName);
        Task<bool> IsOrderStatusBeingUsed(int statusId);
        Task AddNewOrderStatus(OrderStatus orderStatus);
        Task UpdateOrderStatus(OrderStatus orderStatus);
        Task RemoveOrderStatus(OrderStatus orderStatus);
        Task<OrderStatus?> GetDefaultOrderStatus();
        Task RemoveStatusTransitionsByStatusId(int statusId);
    }
}
