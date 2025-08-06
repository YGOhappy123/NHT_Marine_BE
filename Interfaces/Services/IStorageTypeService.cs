using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IStorageTypeService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<StorageType>>> GetAllStorageTypes(BaseQueryObject queryObject);
        Task<ServiceResponse<StorageType?>> GetStorageTypeById(int storageTypeId, int authRoleId);
        Task<ServiceResponse> AddNewStorageType(CreateUpdateStorageTypeDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateStorageType(CreateUpdateStorageTypeDto updateDto, int targetStorageTypeId, int authRoleId);
        Task<ServiceResponse> RemoveStorageType(int targetStorageTypeId, int authRoleId);
    }
}
