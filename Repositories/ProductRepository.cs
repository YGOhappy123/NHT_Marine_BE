using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Statistic;
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
                                query = query.Where(rp =>
                                    rp.ProductItems.Any(pi =>
                                        pi.Inventories.Sum(inv => (int?)inv.Quantity)
                                            - pi.Orders.Where(oi =>
                                                    oi.Order!.OrderStatus!.IsUnfulfilled == false && oi.Order.IsStockReduced == false
                                                )
                                                .Sum(oi => (int?)oi.Quantity)
                                        > 0
                                    )
                                );
                            }
                            else if (value == "False" || value == "false" || value == "0")
                            {
                                query = query.Where(rp =>
                                    !rp.ProductItems.Any(pi =>
                                        pi.Inventories.Sum(inv => (int?)inv.Quantity)
                                            - pi.Orders.Where(oi =>
                                                    oi.Order!.OrderStatus!.IsUnfulfilled == false && oi.Order.IsStockReduced == false
                                                )
                                                .Sum(oi => (int?)oi.Quantity)
                                        > 0
                                    )
                                );
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

        public async Task<(List<RootProduct>, int)> SearchProductsByName(string searchTerm)
        {
            var query = _dbContext
                .RootProducts
                // Include product items, stock and attributes
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Attributes)
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Inventories)
                // Filter by name (ignore Vietnamese diacritic)
                .Where(rp => EF.Functions.Collate(rp.Name, "Latin1_General_CI_AI").Contains(searchTerm))
                .AsQueryable();

            var total = await query.CountAsync();
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
                .Include(rp => rp.CreatedByStaff)
                .SingleOrDefaultAsync(rp => rp.RootProductId == productId);
        }

        public async Task<RootProduct?> GetProductByName(string productName)
        {
            return await _dbContext
                .RootProducts.Where(rp => rp.Name == productName)
                // Include category
                .Include(rp => rp.Category)
                // Include variants and options
                .Include(rp => rp.Variants)
                .ThenInclude(pv => pv.Options)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateProduct(RootProduct product)
        {
            _dbContext.RootProducts.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddProduct(RootProduct product)
        {
            _dbContext.RootProducts.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsProductDeletable(int productId)
        {
            var hasItemsReferences = await _dbContext
                .ProductItems.Where(pi => pi.RootProductId == productId)
                .AnyAsync(pi =>
                    _dbContext.OrderItems.Any(oi => oi.ProductItemId == pi.ProductItemId)
                    || _dbContext.ImportItems.Any(ii => ii.ProductItemId == pi.ProductItemId)
                    || _dbContext.Inventories.Any(i => i.ProductItemId == pi.ProductItemId)
                    || _dbContext.DamageReportItems.Any(dri => dri.ProductItemId == pi.ProductItemId)
                );

            var hasPromotions = await _dbContext.ProductsPromotions.AnyAsync(pp => pp.ProductId == productId);

            return !hasItemsReferences && !hasPromotions;
        }

        public async Task DeleteProductById(int productId)
        {
            var rootProduct = await _dbContext
                .RootProducts
                // Include variants and options
                .Include(rp => rp.Variants)
                .ThenInclude(pv => pv.Options)
                // Include product items and attributes
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.Attributes)
                // Include product items and cart items
                .Include(rp => rp.ProductItems)
                .ThenInclude(pi => pi.CartItems)
                .SingleOrDefaultAsync(rp => rp.RootProductId == productId);

            var productItems = rootProduct!.ProductItems;
            foreach (var item in productItems)
            {
                _dbContext.ProductAttributes.RemoveRange(item.Attributes);
                _dbContext.CartItems.RemoveRange(item.CartItems);
            }

            _dbContext.ProductItems.RemoveRange(productItems);

            foreach (var variant in rootProduct.Variants)
            {
                _dbContext.VariantOptions.RemoveRange(variant.Options);
            }

            _dbContext.ProductVariants.RemoveRange(rootProduct.Variants);
            _dbContext.RootProducts.Remove(rootProduct);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Promotion>> GetProductAvailablePromotions(int productId)
        {
            var currentTime = TimestampHandler.GetNow();

            return await _dbContext
                .Promotions.Where(p =>
                    p.StartDate <= currentTime
                    && p.EndDate >= currentTime
                    && p.IsActive == true
                    && p.Products.Any(pp => pp.ProductId == productId)
                )
                .OrderByDescending(p => p.StartDate)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<DetailedProductItemDto>, int)> GetDetailedProductItems(List<int> productItemIds)
        {
            var query = _dbContext
                .ProductItems.Where(pi => productItemIds.Contains(pi.ProductItemId))
                .Select(pi => new DetailedProductItemDto
                {
                    ProductItemId = pi.ProductItemId,
                    ImageUrl = pi.ImageUrl,
                    Price = pi.Price,
                    Attributes = pi
                        .Attributes.Select(pa => new PartialAttributeDto { Variant = pa.Option!.Variant!.Name, Option = pa.Option.Value })
                        .ToList(),
                    PackingGuide = pi.PackingGuide,
                    RootProduct = new PartialRootProductDto
                    {
                        RootProductId = pi.RootProduct!.RootProductId,
                        Name = pi.RootProduct.Name,
                        Description = pi.RootProduct.Description,
                        ImageUrl = pi.RootProduct.ImageUrl,
                    },
                })
                .AsQueryable();

            var total = await query.CountAsync();

            var productItems = await query.ToListAsync();

            return (productItems, total);
        }

        public async Task<DetailedProductItemDto?> GetDetailedProductItemById(int productItemId)
        {
            return await _dbContext
                .ProductItems.Select(pi => new DetailedProductItemDto
                {
                    ProductItemId = pi.ProductItemId,
                    ImageUrl = pi.ImageUrl,
                    Price = pi.Price,
                    Attributes = pi
                        .Attributes.Select(pa => new PartialAttributeDto { Variant = pa.Option!.Variant!.Name, Option = pa.Option.Value })
                        .ToList(),
                    PackingGuide = pi.PackingGuide,
                    RootProduct = new PartialRootProductDto
                    {
                        RootProductId = pi.RootProduct!.RootProductId,
                        Name = pi.RootProduct.Name,
                        Description = pi.RootProduct.Description,
                        ImageUrl = pi.RootProduct.ImageUrl,
                    },
                })
                .SingleOrDefaultAsync(pi => pi.ProductItemId == productItemId);
        }

        public async Task<ProductItem?> GetProductItemById(int productItemId)
        {
            return await _dbContext.ProductItems.SingleOrDefaultAsync(pi => pi.ProductItemId == productItemId);
        }

        public async Task<int> GetProductItemCurrentStock(int productItemId)
        {
            var totalInventories = await _dbContext.Inventories.Where(i => i.ProductItemId == productItemId).SumAsync(i => i.Quantity);
            var totalPendingItems = await _dbContext
                .OrderItems.Where(oi =>
                    oi.Order!.OrderStatus!.IsUnfulfilled == false && oi.Order.IsStockReduced == false && oi.ProductItemId == productItemId
                )
                .SumAsync(oi => oi.Quantity);

            return Math.Max(totalInventories - totalPendingItems, 0);
        }

        public async Task UpdateProductItem(ProductItem productItem)
        {
            _dbContext.ProductItems.Update(productItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ProductStatisticDto>> GetProductStatisticInTimeRange(DateTime startTime, DateTime endTime, int productId)
        {
            var matchingOrderIds = await _dbContext
                .Orders.Where(o => o.CreatedAt >= startTime && o.CreatedAt < endTime && o.OrderStatus!.IsAccounted == true)
                .Select(o => o.OrderId)
                .ToListAsync();

            var matchingOrderItems = await _dbContext
                .OrderItems.Where(oi =>
                    matchingOrderIds.Contains((int)oi.OrderId!) && oi.ProductItem!.RootProduct!.RootProductId == productId
                )
                .GroupBy(oi => oi.ProductItemId)
                .Select(g => new ProductStatisticDto
                {
                    ProductItemId = (int)g.Key!,
                    TotalUnits = g.Sum(oi => oi.Quantity),
                    TotalSales = g.Sum(oi => oi.Price * oi.Quantity),
                })
                .ToListAsync();

            return matchingOrderItems;
        }
    }
}
