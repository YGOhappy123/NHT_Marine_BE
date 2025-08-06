using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IDeliveryServiceService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<DeliveryService>>> GetAllDeliveryServices(BaseQueryObject queryObject);
        Task<ServiceResponse<DeliveryService?>> GetDeliveryServiceById(int serviceId, int authRoleId);
        Task<ServiceResponse> AddNewDeliveryService(CreateUpdateDeliveryServiceDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateDeliveryService(CreateUpdateDeliveryServiceDto updateDto, int targetServiceId, int authRoleId);
        Task<ServiceResponse> RemoveDeliveryService(int targetServiceId, int authRoleId);
    }
}
