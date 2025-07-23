using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoryRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Category> ApplyFilters(IQueryable<Category> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "name":
                            query = query.Where(c => c.Name.Contains(value));
                            break;
                        default:
                            query = query.Where(c => EF.Property<string>(c, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Category> ApplySorting(IQueryable<Category> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(c => EF.Property<object>(c, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(c => EF.Property<object>(c, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<Category>, int)> GetAllCategories(BaseQueryObject queryObject)
        {
            var query = _dbContext.Categories.Include(c => c.ParentCategory).AsQueryable();

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

            var categories = await query.ToListAsync();

            return (categories, total);
        }
    }
}
