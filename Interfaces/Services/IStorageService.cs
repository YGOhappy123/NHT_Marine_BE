using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IStorageService
    {
        Task<ServiceResponse<List<StorageDto>>> GetAllStorages(BaseQueryObject queryObject);
        Task<ServiceResponse> ChangeInventoryLocation(ChangeInventoryLocationDto changeInventoryDto, int storageId, int authRoleId);
        Task<ServiceResponse> ChangeInventoryVariant(ChangeInventoryVariantDto changeInventoryDto, int storageId, int authRoleId);
        Task<ServiceResponse<Storage?>> GetStorageById(int storageId, int authRoleId);
        Task<ServiceResponse> AddNewStorage(CreateUpdateStorageDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateStorage(CreateUpdateStorageDto updateDto, int storageId, int authRoleId);
        Task<ServiceResponse> RemoveStorage(int storageId, int authRoleId);
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
    }
}
