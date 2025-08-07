using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Interfaces.Repositories;

namespace NHT_Marine_BE.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public StorageRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<List<ProductInventoryDto>> GetProductItemInventories(List<int> productItemIds)
        {
            return await _dbContext
                .Inventories.Where(i => productItemIds.Contains((int)i.ProductItemId!) && i.Quantity > 0)
                .Include(i => i.Storage)
                .GroupBy(i => i.ProductItemId)
                .Select(g => new ProductInventoryDto
                {
                    ProductItemId = (int)g.Key!,
                    Storages = g.Select(i => new ProductItemStorageDto
                        {
                            StorageId = (int)i.StorageId!,
                            Storage = i.Storage == null ? null : new StorageDto { StorageId = i.Storage.StorageId, Name = i.Storage.Name },
                            Quantity = i.Quantity,
                        })
                        .ToList(),
                })
                .ToListAsync();
        }
    }
}
