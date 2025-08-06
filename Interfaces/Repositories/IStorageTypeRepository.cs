using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IStorageTypeRepository
    {
        Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<StorageType>, int)> GetAllStorageTypes(BaseQueryObject queryObject);
        Task<StorageType?> GetStorageTypeById(int storageTypeId);
        Task<StorageType?> GetStorageTypeByName(string storageTypeName);
        Task<bool> IsStorageTypeBeingUsed(int storageTypeId);
        Task AddStorageType(StorageType storageType);
        Task UpdateStorageType(StorageType storageType);
        Task DeleteStorageType(StorageType storageType);
    }
}
