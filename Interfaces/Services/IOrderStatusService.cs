using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IOrderStatusService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<OrderStatus>>> GetAllOrderStatuses(BaseQueryObject queryObject);
        Task<ServiceResponse<OrderStatus?>> GetOrderStatusById(int statusId, int authRoleId);
        Task<ServiceResponse> AddNewOrderStatus(CreateUpdateOrderStatusDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateOrderStatus(CreateUpdateOrderStatusDto updateDto, int targetStatusId, int authRoleId);
        Task<ServiceResponse> RemoveOrderStatus(int targetStatusId, int authRoleId);
        Task<ServiceResponse<List<StatusTransitionGroupDto>>> GetAllTransitions(BaseQueryObject queryObject);
        Task<ServiceResponse> AddNewTransition(CreateUpdateStatusTransitionDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateTransition(CreateUpdateStatusTransitionDto updateDto, int transitionId, int authRoleId);
        Task<ServiceResponse> RemoveTransition(int transitionId, int authRoleId);
    }
}
