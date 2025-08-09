using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IStorageRepository
    {
        Task<(List<Storage>, int)> GetAllStorages(BaseQueryObject queryObject);
        Task<List<ProductInventoryDto>> GetProductItemInventories(List<int> productItemIds);
        Task<Inventory?> GetInventoryByStorageAndProductItemId(int storageId, int productItemId);
        Task AddInventory(Inventory inventory);
        Task UpdateInventory(Inventory inventory);
        Task DeleteInventory(Inventory inventory);
    }
}
