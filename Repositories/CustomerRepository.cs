using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CustomerRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Customer> ApplyFilters(IQueryable<Customer> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "startTime":
                            query = query.Where(cus => cus.CreatedAt >= DateTime.Parse(value));
                            break;
                        case "endTime":
                            query = query.Where(f => f.CreatedAt <= TimestampHandler.GetEndOfTimeByType(DateTime.Parse(value), "daily"));
                            break;
                        case "email":
                            query = query.Where(cus => cus.Email!.Contains(value));
                            break;
                        case "name":
                        case "fullName":
                            query = query.Where(cus => cus.FullName.Contains(value));
                            break;
                        default:
                            query = query.Where(cus => EF.Property<string>(cus, filter.Key.CapitalizeSingleWord()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Customer> ApplySorting(IQueryable<Customer> query, Dictionary<string, string> sort)
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

        public async Task<Customer?> GetCustomerById(int customerId)
        {
            return await _dbContext
                .Customers.Include(c => c.Account)
                .Where(c => c.Account!.IsActive && c.CustomerId == customerId)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetCustomerByAccountId(int accountId)
        {
            return await _dbContext.Customers.SingleOrDefaultAsync(c => c.AccountId == accountId);
        }

        public async Task<Customer?> GetCustomerByName(string name)
        {
            return await _dbContext.Customers.SingleOrDefaultAsync(c => c.FullName == name);
        }

        public async Task<Customer?> GetCustomerByEmail(string email)
        {
            return await _dbContext
                .Customers.Include(c => c.Account)
                .Where(c => c.Account!.IsActive && c.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<Customer>, int)> GetAllCustomers(BaseQueryObject queryObject)
        {
            var query = _dbContext.Customers.Include(cus => cus.Account).AsQueryable();

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

        public async Task AddCustomer(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
