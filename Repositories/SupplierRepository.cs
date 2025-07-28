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
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public SupplierRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Supplier> ApplyFilters(IQueryable<Supplier> query, Dictionary<string, object> filters)
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

        private IQueryable<Supplier> ApplySorting(IQueryable<Supplier> query, Dictionary<string, string> sort)
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

        public async Task<(List<Supplier>, int)> GetAllSuppliers(BaseQueryObject queryObject)
        {
            var query = _dbContext.Suppliers.AsQueryable();

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

        public async Task<Supplier?> GetSupplierById(int supplierId)
        {
            return await _dbContext.Suppliers.SingleOrDefaultAsync(s => s.SupplierId == supplierId);
        }

        public async Task<Supplier?> GetSupplierByName(string supplierName)
        {
            return await _dbContext.Suppliers.Where(s => s.Name == supplierName).SingleOrDefaultAsync();
        }

        public async Task<bool> IsSSupplierBeingUsed(int supplierId)
        {
            return await _dbContext.ProductImports.AnyAsync(sto => sto.SupplierId == supplierId);
        }

        public async Task AddSupplier(Supplier supplier)
        {
            _dbContext.Suppliers.Add(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSupplier(Supplier supplier)
        {
            _dbContext.Suppliers.Update(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSupplier(Supplier supplier)
        {
            _dbContext.Suppliers.Remove(supplier);
            await _dbContext.SaveChangesAsync();
        }
    }
}
