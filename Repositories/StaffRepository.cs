using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public StaffRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Staff> ApplyFilters(IQueryable<Staff> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "startTime":
                            query = query.Where(st => st.CreatedAt >= DateTime.Parse(value));
                            break;
                        case "endTime":
                            query = query.Where(f => f.CreatedAt <= TimestampHandler.GetEndOfTimeByType(DateTime.Parse(value), "daily"));
                            break;
                        case "email":
                            query = query.Where(st => st.Email!.Contains(value));
                            break;
                        case "name":
                        case "fullName":
                            query = query.Where(st => st.FullName.Contains(value));
                            break;
                        default:
                            query = query.Where(st => EF.Property<string>(st, filter.Key.CapitalizeSingleWord()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Staff> ApplySorting(IQueryable<Staff> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(st => EF.Property<object>(st, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(st => EF.Property<object>(st, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<Staff?> GetStaffById(int staffId)
        {
            return await _dbContext
                .Staffs.Include(st => st.Account)
                .Include(st => st.CreatedByStaff)
                .Include(st => st.Role)
                .ThenInclude(sr => sr.Permissions)
                .ThenInclude(rp => rp.Permission)
                .Where(st => st.Account!.IsActive && st.StaffId == staffId)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff?> GetStaffByIdIncludeInactive(int staffId)
        {
            return await _dbContext.Staffs.Include(st => st.Account).Where(st => st.StaffId == staffId).FirstOrDefaultAsync();
        }

        public async Task<Staff?> GetStaffByAccountId(int accountId)
        {
            return await _dbContext
                .Staffs.Include(st => st.Account)
                .Include(st => st.CreatedByStaff)
                .Include(st => st.Role)
                .ThenInclude(sr => sr.Permissions)
                .ThenInclude(rp => rp.Permission)
                .SingleOrDefaultAsync(st => st.AccountId == accountId);
        }

        public async Task<Staff?> GetStaffByEmail(string email)
        {
            return await _dbContext
                .Staffs.Include(st => st.Account)
                .Include(st => st.CreatedByStaff)
                .Include(st => st.Role)
                .ThenInclude(sr => sr.Permissions)
                .ThenInclude(rp => rp.Permission)
                .Where(st => st.Account!.IsActive && st.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff?> GetStaffByEmailIncludeInactive(string email)
        {
            return await _dbContext
                .Staffs.Include(st => st.Account)
                .Include(st => st.CreatedByStaff)
                .Include(st => st.Role)
                .Where(st => st.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<Staff>, int)> GetAllStaffs(BaseQueryObject queryObject)
        {
            var query = _dbContext.Staffs.Include(st => st.Account).Include(st => st.CreatedByStaff).Include(st => st.Role).AsQueryable();

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

            var customers = await query.ToListAsync();

            return (customers, total);
        }

        public async Task AddStaff(Staff staff)
        {
            _dbContext.Staffs.Add(staff);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStaff(Staff staff)
        {
            _dbContext.Staffs.Update(staff);
            await _dbContext.SaveChangesAsync();
        }
    }
}
