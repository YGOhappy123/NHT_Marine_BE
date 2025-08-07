using NHT_Marine_BE.Data.Dtos.Order;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IStorageRepository
    {
        Task<List<ProductInventoryDto>> GetProductItemInventories(List<int> productItemIds);
    }
}
