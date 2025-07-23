using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ProductRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<RootProduct> ApplyFilters(IQueryable<RootProduct> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "name":
                            query = query.Where(rp => rp.Name.Contains(value));
                            break;
                        case "categories":
                            var categoryIds = JsonSerializer.Deserialize<List<int>>(filter.Value.ToString() ?? "[]");
                            query = query.Where(rp => categoryIds!.Contains((int)rp.CategoryId!));
                            break;
                        case "minPrice":
                            query = query.Where(rp => rp.ProductItems.Any(item => item.Price >= Convert.ToDecimal(value)));
                            break;
                        case "maxPrice":
                            query = query.Where(rp => rp.ProductItems.Any(item => item.Price <= Convert.ToDecimal(value)));
                            break;
                        case "inStock":
                            if (value == "True" || value == "true" || value == "1")
                            {
                                query = query.Where(rp => rp.ProductItems.Any(pi => pi.Inventories.Sum(inv => (int?)inv.Quantity) > 0));
                            }
                            else if (value == "False" || value == "false" || value == "0")
                            {
                                query = query.Where(rp => !rp.ProductItems.Any(pi => pi.Inventories.Sum(inv => (int?)inv.Quantity) > 0));
                            }
                            break;
                        default:
                            query = query.Where(rp => EF.Property<string>(rp, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<RootProduct> ApplySorting(IQueryable<RootProduct> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(rp => EF.Property<object>(rp, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(rp => EF.Property<object>(rp, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<RootProduct>, int)> GetAllProducts(BaseQueryObject queryObject)
        {
            var query = _dbContext
                .RootProducts
                // Include category
                .Include(rp => rp.Category)
                // Include product items, stock and attributes
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Attributes)
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Inventories)
                // Include promotions
                .Include(rp => rp.Promotions)
                .ThenInclude(pp => pp.Promotion)
                // Include creator
                .Include(rp => rp.CreatedByStaff)
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

            var products = await query.ToListAsync();

            return (products, total);
        }

        public async Task<RootProduct?> GetProductById(int productId)
        {
            return await _dbContext
                .RootProducts
                // Include category
                .Include(rp => rp.Category)
                // Include variants and options
                .Include(rp => rp.Variants)
                .ThenInclude(pv => pv.Options)
                // Include product items, stock and attributes
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Attributes)
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Inventories)
                // Include promotions
                .Include(rp => rp.Promotions)
                .ThenInclude(pp => pp.Promotion)
                // Include creator
                .Include(rp => rp.CreatedByStaff)
                .SingleOrDefaultAsync(rp => rp.RootProductId == productId);
        }
    }
}
