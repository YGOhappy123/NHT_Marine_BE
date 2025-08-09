using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IStorageService
    {
        Task<ServiceResponse<List<StorageDto>>> GetAllStorages(BaseQueryObject queryObject);
        Task<ServiceResponse> ChangeInventoryLocation(ChangeInventoryLocationDto changeInventoryDto, int storageId, int authRoleId);
        Task<ServiceResponse> ChangeInventoryVariant(ChangeInventoryVariantDto changeInventoryDto, int storageId, int authRoleId);
    }
}
