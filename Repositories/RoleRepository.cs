using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public RoleRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<StaffRole> ApplyFilters(IQueryable<StaffRole> query, Dictionary<string, object> filters)
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
                        case "permissions":
                            var permissionIds = JsonSerializer.Deserialize<List<int>>(filter.Value.ToString() ?? "[]");
                            query = query.Where(sr =>
                                permissionIds!.All(permissionId => sr.Permissions.Any(rp => rp.PermissionId == permissionId))
                            );
                            break;
                        default:
                            query = query.Where(sr => EF.Property<string>(sr, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<StaffRole> ApplySorting(IQueryable<StaffRole> query, Dictionary<string, string> sort)
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

        public async Task<(List<StaffRole>, int)> GetAllRoles(BaseQueryObject queryObject)
        {
            var query = _dbContext.StaffRoles.Include(sr => sr!.Permissions).ThenInclude(rp => rp.Permission).AsQueryable();

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

        public async Task<StaffRole?> GetRoleById(int roleId)
        {
            return await _dbContext
                .StaffRoles.Include(sr => sr!.Permissions)
                .ThenInclude(rp => rp.Permission)
                .SingleOrDefaultAsync(sr => sr.RoleId == roleId);
        }

        public async Task<StaffRole?> GetRoleByName(string roleName)
        {
            return await _dbContext.StaffRoles.Where(sr => sr.Name == roleName).SingleOrDefaultAsync();
        }

        public async Task<bool> IsRoleBeingUsed(int roleId)
        {
            return await _dbContext.Staffs.AnyAsync(s => s.RoleId == roleId);
        }

        public async Task AddRole(StaffRole role)
        {
            _dbContext.StaffRoles.Add(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRole(StaffRole role)
        {
            _dbContext.StaffRoles.Update(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRole(StaffRole role)
        {
            _dbContext.StaffRoles.Remove(role);
            await _dbContext.SaveChangesAsync();
        }
    }
}
