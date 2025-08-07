using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public OrderRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Order> ApplyFilters(IQueryable<Order> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        default:
                            query = query.Where(o => EF.Property<string>(o, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Order> ApplySorting(IQueryable<Order> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(o => EF.Property<object>(o, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(o => EF.Property<object>(o, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<Order>, int)> GetAllOrders(BaseQueryObject queryObject)
        {
            var query = _dbContext
                .Orders.Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.Coupon)
                .Include(o => o.Items)
                .AsQueryable();

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

            var orders = await query.ToListAsync();

            return (orders, total);
        }

        public async Task<(List<Order>, int)> GetCustomerOrders(BaseQueryObject queryObject, int customerId)
        {
            var query = _dbContext
                .Orders.Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderStatus)
                .Include(o => o.Coupon)
                .Include(o => o.Items)
                .AsQueryable();

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

            var orders = await query.ToListAsync();

            return (orders, total);
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            return await _dbContext
                .Orders.Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.Coupon)
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<OrderStatus?> GetDefaultOrderStatus()
        {
            return await _dbContext.OrderStatuses.Where(os => os.IsDefaultState == true).FirstOrDefaultAsync();
        }

        public async Task<List<StatusTransition>> GetStatusTransitions(int orderStatusId)
        {
            return await _dbContext
                .StatusTransitions.Where(st => st.FromStatusId == orderStatusId)
                .Include(st => st.ToStatus)
                .ToListAsync();
        }

        public async Task<List<OrderStatusUpdateLog>> GetStatusUpdateLogs(int orderId)
        {
            return await _dbContext
                .OrderStatusUpdateLogs.Where(sul => sul.OrderId == orderId)
                .Include(sul => sul.Status)
                .Include(sul => sul.UpdatedByStaff)
                .OrderByDescending(sul => sul.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddOrder(Order order)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ProcessOrderInventory(AcceptOrderDto acceptOrderDto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var product in acceptOrderDto.Inventories)
                {
                    foreach (var storage in product.Storages)
                    {
                        var inventoryItem = await _dbContext
                            .Inventories.Where(i => i.ProductItemId == product.ProductItemId && i.StorageId == storage.StorageId)
                            .FirstOrDefaultAsync();

                        if (inventoryItem == null)
                        {
                            throw new Exception("ITEM_NOT_FOUND");
                        }

                        if (inventoryItem.Quantity < storage.Quantity)
                        {
                            throw new Exception("ITEM_EXCEED_STOCK");
                        }

                        inventoryItem.Quantity -= storage.Quantity;
                        if (inventoryItem.Quantity == 0)
                        {
                            _dbContext.Inventories.Remove(inventoryItem);
                        }
                        else
                        {
                            _dbContext.Inventories.Update(inventoryItem);
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddStatusUpdateLog(OrderStatusUpdateLog log)
        {
            _dbContext.OrderStatusUpdateLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
