using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ImportRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<ProductImport> ApplyFilters(IQueryable<ProductImport> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "isDistributed":
                            if (value == "True" || value == "true" || value == "1")
                            {
                                query = query.Where(pi => pi.IsDistributed == true);
                            }
                            else if (value == "False" || value == "false" || value == "0")
                            {
                                query = query.Where(pi => pi.IsDistributed == false);
                            }
                            break;
                        default:
                            query = query.Where(pi => EF.Property<string>(pi, filter.Key.CapitalizeSingleWord()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<ProductImport> ApplySorting(IQueryable<ProductImport> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(pi => EF.Property<object>(pi, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(pi => EF.Property<object>(pi, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<ProductImport>, int)> GetAllProductImports(BaseQueryObject queryObject)
        {
            var query = _dbContext
                .ProductImports.Include(pi => pi.Supplier)
                .Include(pi => pi.TrackedByStaff)
                .ThenInclude(s => s!.Account)
                .Include(pi => pi.Items)
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

            var imports = await query.ToListAsync();

            return (imports, total);
        }

        public async Task<ProductImport?> GetProductImportById(int importId)
        {
            return await _dbContext
                .ProductImports.Include(pi => pi.Supplier)
                .Include(pi => pi.TrackedByStaff)
                .ThenInclude(s => s!.Account)
                .Include(pi => pi.Items)
                .SingleOrDefaultAsync(pi => pi.ImportId == importId);
        }

        public async Task<List<ProductImport>> GetAllImportsInTimeRange(DateTime startTime, DateTime endTime)
        {
            return await _dbContext.ProductImports.Where(od => od.TrackedAt >= startTime && od.TrackedAt < endTime).ToListAsync();
        }

        public async Task AddProductImport(ProductImport productImport)
        {
            _dbContext.ProductImports.Add(productImport);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductImport(ProductImport productImport)
        {
            _dbContext.ProductImports.Update(productImport);
            await _dbContext.SaveChangesAsync();
        }
    }
}
