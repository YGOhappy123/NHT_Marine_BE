using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public PromotionRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Promotion> ApplyFilters(IQueryable<Promotion> query, Dictionary<string, object> filters)
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

        private IQueryable<Promotion> ApplySorting(IQueryable<Promotion> query, Dictionary<string, string> sort)
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

        public async Task<(List<Promotion>, int)> GetAllPromotions(BaseQueryObject queryObject)
        {
            var query = _dbContext
                .Promotions.Include(p => p.CreatedByStaff)
                .Include(p => p.Products)
                .ThenInclude(pp => pp.Product)
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

            var promotions = await query.ToListAsync();

            return (promotions, total);
        }

        public async Task<Promotion?> GetPromotionByName(string promotionName)
        {
            return await _dbContext.Promotions.Where(pr => pr.Name == promotionName).SingleOrDefaultAsync();
        }

        public async Task<Promotion?> GetPromotionById(int promotionId)
        {
            return await _dbContext
                .Promotions.Include(p => p.CreatedByStaff)
                .Include(p => p.Products)
                .ThenInclude(pp => pp.Product)
                .Include(p => p.Products)
                .ThenInclude(pp => pp.Product)
                .SingleOrDefaultAsync(p => p.PromotionId == promotionId);
        }

        public async Task AddPromotion(Promotion promotion)
        {
            _dbContext.Promotions.Add(promotion);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePromotion(Promotion promotion)
        {
            _dbContext.Promotions.Update(promotion);
            await _dbContext.SaveChangesAsync();
        }
    }
}
