using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public OrderStatusRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<OrderStatus> ApplyFilters(IQueryable<OrderStatus> query, Dictionary<string, object> filters)
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

        private IQueryable<OrderStatus> ApplySorting(IQueryable<OrderStatus> query, Dictionary<string, string> sort)
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

        public async Task<(List<OrderStatus>, int)> GetAllOrderStatuses(BaseQueryObject queryObject)
        {
            var query = _dbContext.OrderStatuses.AsQueryable();

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

        public async Task<OrderStatus?> GetOrderStatusById(int statusId)
        {
            return await _dbContext.OrderStatuses.SingleOrDefaultAsync(s => s.StatusId == statusId);
        }

        public async Task<OrderStatus?> GetOrderStatusByName(string statusName)
        {
            return await _dbContext.OrderStatuses.Where(s => s.Name == statusName).SingleOrDefaultAsync();
        }

        public async Task<bool> IsOrderStatusBeingUsed(int statusId)
        {
            return await _dbContext.OrderStatusUpdateLogs.AnyAsync(sto => sto.StatusId == statusId)
                || await _dbContext.Orders.AnyAsync(o => o.OrderStatusId == statusId);
        }

        public async Task AddNewOrderStatus(OrderStatus orderStatus)
        {
            _dbContext.OrderStatuses.Add(orderStatus);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderStatus(OrderStatus orderStatus)
        {
            _dbContext.OrderStatuses.Update(orderStatus);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveOrderStatus(OrderStatus orderStatus)
        {
            await RemoveStatusTransitionsByStatusId(orderStatus.StatusId);
            _dbContext.OrderStatuses.Remove(orderStatus);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderStatus?> GetDefaultOrderStatus()
        {
            return await _dbContext.OrderStatuses.FirstOrDefaultAsync(os => os.IsDefaultState);
        }

        public async Task RemoveStatusTransitionsByStatusId(int statusId)
        {
            var transitions = await _dbContext
                .StatusTransitions.Where(st => st.FromStatusId == statusId || st.ToStatusId == statusId)
                .ToListAsync();
            _dbContext.StatusTransitions.RemoveRange(transitions);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckValidStatusTransition(int fromStatusId, int toStatusId)
        {
            return await _dbContext.StatusTransitions.AnyAsync(st => st.FromStatusId == fromStatusId && st.ToStatusId == toStatusId);
        }
    }
}
