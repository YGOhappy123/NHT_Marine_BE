using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class DeliveryServiceRepository : IDeliveryServiceRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public DeliveryServiceRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<DeliveryService> ApplyFilters(IQueryable<DeliveryService> query, Dictionary<string, object> filters)
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

        private IQueryable<DeliveryService> ApplySorting(IQueryable<DeliveryService> query, Dictionary<string, string> sort)
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

        public async Task<(List<DeliveryService>, int)> GetAllDeliveryServices(BaseQueryObject queryObject)
        {
            var query = _dbContext.DeliveryServices.AsQueryable();

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

        public async Task<DeliveryService?> GetDeliveryServiceById(int serviceId)
        {
            return await _dbContext.DeliveryServices.SingleOrDefaultAsync(s => s.ServiceId == serviceId);
        }

        public async Task<DeliveryService?> GetDeliveryServiceByName(string serviceName)
        {
            return await _dbContext.DeliveryServices.Where(s => s.Name == serviceName).SingleOrDefaultAsync();
        }

        public async Task<bool> IsDeliveryServiceBeingUsed(int serviceId)
        {
            return await _dbContext.OrderDeliveries.AnyAsync(sto => sto.DeliveryServiceId == serviceId);
        }

        public async Task AddNewDeliveryService(DeliveryService deliveryService)
        {
            _dbContext.DeliveryServices.Add(deliveryService);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDeliveryService(DeliveryService deliveryService)
        {
            _dbContext.DeliveryServices.Update(deliveryService);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveDeliveryService(DeliveryService deliveryService)
        {
            _dbContext.DeliveryServices.Remove(deliveryService);
            await _dbContext.SaveChangesAsync();
        }
    }
}
