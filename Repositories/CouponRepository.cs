using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CouponRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Coupon> ApplyFilters(IQueryable<Coupon> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        default:
                            query = query.Where(cus => EF.Property<string>(cus, filter.Key.CapitalizeSingleWord()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Coupon> ApplySorting(IQueryable<Coupon> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(cus => EF.Property<object>(cus, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(cus => EF.Property<object>(cus, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<Coupon>, int)> GetAllCoupons(BaseQueryObject queryObject)
        {
            var query = _dbContext.Coupons.Include(c => c.CreatedByStaff).AsQueryable();

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

            var coupons = await query.ToListAsync();

            return (coupons, total);
        }

        public async Task<Coupon?> GetCouponById(int couponId)
        {
            return await _dbContext
                .Coupons.Include(c => c.CreatedByStaff)
                .Include(c => c.CreatedByStaff)
                .SingleOrDefaultAsync(c => c.CouponId == couponId);
        }

        public async Task<Coupon?> GetCouponByName(string couponName)
        {
            return await _dbContext.Coupons.Where(c => c.Code == couponName).FirstOrDefaultAsync();
        }

        public async Task AddCoupon(Coupon coupon)
        {
            _dbContext.Coupons.Add(coupon);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCoupon(Coupon coupon)
        {
            _dbContext.Coupons.Update(coupon);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Coupon?> GetCouponByCode(string code)
        {
            return await _dbContext.Coupons.SingleOrDefaultAsync(c => c.Code == code);
        }

        public async Task<int> CountCouponUsage(int couponId)
        {
            return await _dbContext.Orders.Where(o => o.OrderStatus!.IsUnfulfilled == false && o.CouponId == couponId).CountAsync();
        }

        public async Task<bool> CheckCustomerCouponUsed(int couponId, int customerId)
        {
            return await _dbContext
                .Orders.Where(o => o.OrderStatus!.IsUnfulfilled == false && o.CouponId == couponId && o.CustomerId == customerId)
                .AnyAsync();
        }
    }
}
