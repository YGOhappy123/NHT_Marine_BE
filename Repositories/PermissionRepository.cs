using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public PermissionRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<AppPermission> ApplyFilters(IQueryable<AppPermission> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "name":
                            query = query.Where(ap => ap.Name.Contains(value));
                            break;
                        default:
                            query = query.Where(ap => EF.Property<string>(ap, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<AppPermission> ApplySorting(IQueryable<AppPermission> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(ap => EF.Property<object>(ap, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(ap => EF.Property<object>(ap, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<AppPermission>, int)> GetAllPermissions(BaseQueryObject queryObject)
        {
            var query = _dbContext.AppPermissions.AsQueryable();

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

            var permissions = await query.ToListAsync();

            return (permissions, total);
        }
    }
}
