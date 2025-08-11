using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public StorageRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Storage> ApplyFilters(IQueryable<Storage> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        default:
                            query = query.Where(s => EF.Property<string>(s, filter.Key.CapitalizeSingleWord()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Storage> ApplySorting(IQueryable<Storage> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(s => EF.Property<object>(s, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(s => EF.Property<object>(s, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<Storage>, int)> GetAllStorages(BaseQueryObject queryObject)
        {
            var query = _dbContext.Storages.Include(s => s.Type).Include(s => s.ProductItems).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Filter))
            {
                var parsedFilter = JsonSerializer.Deserialize<Dictionary<string, object>>(queryObject.Filter);
                query = ApplyFilters(query, parsedFilter!);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.Sort))
            {
                var parsedSort = JsonSerializer.Deserialize<Dictionary<string, string>>(queryObject.Sort);
                query = ApplySorting(query, parsedSort!);
            }

            var total = await query.CountAsync();

            if (queryObject.Skip.HasValue)
                query = query.Skip(queryObject.Skip.Value);

            if (queryObject.Limit.HasValue)
                query = query.Take(queryObject.Limit.Value);

            var storages = await query.ToListAsync();

            return (storages, total);
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

        public async Task<Inventory?> GetInventoryByStorageAndProductItemId(int storageId, int productItemId)
        {
            return await _dbContext
                .Inventories.Where(iv => iv.StorageId == storageId && iv.ProductItemId == productItemId)
                .FirstOrDefaultAsync();
        }

        public async Task AddInventory(Inventory inventory)
        {
            _dbContext.Inventories.Add(inventory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInventory(Inventory inventory)
        {
            _dbContext.Inventories.Update(inventory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInventory(Inventory inventory)
        {
            _dbContext.Inventories.Remove(inventory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Storage?> GetStorageById(int storageId)
        {
            return await _dbContext.Storages.SingleOrDefaultAsync(st => st.StorageId == storageId);
        }

        public async Task<Storage?> GetStorageByName(string storageName)
        {
            return await _dbContext.Storages.Where(st => st.Name == storageName).SingleOrDefaultAsync();
        }

        public async Task<bool> IsStorageBeingUsed(int storageId)
        {
            return await _dbContext.ProductDamageReports.AnyAsync(sto => sto.StorageId == storageId)
                || await _dbContext.Inventories.AnyAsync(iv => iv.StorageId == storageId);
        }

        public async Task AddStorage(Storage storage)
        {
            _dbContext.Storages.Add(storage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStorage(Storage storage)
        {
            _dbContext.Storages.Update(storage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteStorage(Storage storage)
        {
            _dbContext.Storages.Remove(storage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> VerifyPermission(int authRoleId, string permission)
        {
            return await _dbContext.RolesPermissions.AnyAsync(rp =>
                rp.RoleId == authRoleId && rp.Permission != null && rp.Permission.Code == permission
            );
        }
    }
}
