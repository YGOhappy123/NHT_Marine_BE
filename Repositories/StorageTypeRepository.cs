using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class StorageTypeRepository : IStorageTypeRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public StorageTypeRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<StorageType> ApplyFilters(IQueryable<StorageType> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "name":
                            query = query.Where(sr => sr.Name.Contains(value));
                            break;
                        default:
                            query = query.Where(sr => EF.Property<string>(sr, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<StorageType> ApplySorting(IQueryable<StorageType> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(sr => EF.Property<object>(sr, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(sr => EF.Property<object>(sr, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<bool> VerifyPermission(int authRoleId, string permission)
        {
            return await _dbContext.RolesPermissions.AnyAsync(rp =>
                rp.RoleId == authRoleId && rp.Permission != null && rp.Permission.Code == permission
            );
        }

        public async Task<(List<StorageType>, int)> GetAllStorageTypes(BaseQueryObject queryObject)
        {
            var query = _dbContext.StorageTypes.AsQueryable();

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

            var roles = await query.ToListAsync();

            return (roles, total);
        }

        public async Task<StorageType?> GetStorageTypeById(int storageTypeId)
        {
            return await _dbContext.StorageTypes.SingleOrDefaultAsync(st => st.TypeId == storageTypeId);
        }

        public async Task<StorageType?> GetStorageTypeByName(string storageTypeName)
        {
            return await _dbContext.StorageTypes.Where(st => st.Name == storageTypeName).SingleOrDefaultAsync();
        }

        public async Task<bool> IsStorageTypeBeingUsed(int storageTypeId)
        {
            return await _dbContext.ProductDamageReports.AnyAsync(pdr => pdr.TypeId == storageTypeId);
        }

        public async Task AddStorageType(StorageType storageType)
        {
            _dbContext.StorageTypes.Add(storageType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStorageType(StorageType storageType)
        {
            _dbContext.StorageTypes.Update(storageType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteStorageType(StorageType storageType)
        {
            _dbContext.StorageTypes.Remove(storageType);
            await _dbContext.SaveChangesAsync();
        }
    }
}
