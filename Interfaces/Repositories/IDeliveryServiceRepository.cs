using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IDeliveryServiceRepository
    {
        Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<DeliveryService>, int)> GetAllDeliveryServices(BaseQueryObject queryObject);
        Task<DeliveryService?> GetDeliveryServiceById(int serviceId);
        Task<DeliveryService?> GetDeliveryServiceByName(string serviceName);
        Task<bool> IsDeliveryServiceBeingUsed(int serviceId);
        Task AddNewDeliveryService(DeliveryService deliveryService);
        Task UpdateDeliveryService(DeliveryService deliveryService);
        Task RemoveDeliveryService(DeliveryService deliveryService);
        Task<DeliveryService?> GetDeliveryServiceByContactPhone(string contactPhone);
    }
}
