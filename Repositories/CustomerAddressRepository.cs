using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CustomerAddressRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<CustomerAddress> ApplyFilters(IQueryable<CustomerAddress> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        default:
                            query = query.Where(ca => EF.Property<string>(ca, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<CustomerAddress> ApplySorting(IQueryable<CustomerAddress> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(ca => EF.Property<object>(ca, order.Key.CapitalizeSingleWord()))
                        : query.OrderByDescending(ca => EF.Property<object>(ca, order.Key.CapitalizeSingleWord()));
            }

            return query;
        }

        public async Task<(List<CustomerAddress>, int)> GetCustomerAddresses(BaseQueryObject queryObject, int customerId)
        {
            var query = _dbContext.CustomerAddresses.Where(o => o.CustomerId == customerId).Include(o => o.Customer).AsQueryable();

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

            var addresses = await query.ToListAsync();

            return (addresses, total);
        }

        public async Task<CustomerAddress?> GetCustomerAddressById(int addressId)
        {
            return await _dbContext.CustomerAddresses.SingleOrDefaultAsync(ca => ca.AddressId == addressId);
        }

        public async Task<CustomerAddress?> GetCustomerAddressExactMatch(
            string recipientName,
            string phoneNumber,
            string city,
            string district,
            string ward,
            string addressLine,
            int customerId
        )
        {
            return await _dbContext
                .CustomerAddresses.Where(ca =>
                    ca.RecipientName == recipientName
                    && ca.PhoneNumber == phoneNumber
                    && ca.City == city
                    && ca.District == district
                    && ca.Ward == ward
                    && ca.AddressLine == addressLine
                    && ca.CustomerId == customerId
                )
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerAddress?> GetCustomerDefaultAddress(int customerId)
        {
            return await _dbContext
                .CustomerAddresses.Where(ca => ca.CustomerId == customerId && ca.IsDefault == true)
                .FirstOrDefaultAsync();
        }

        public async Task AddCustomerAddress(CustomerAddress customerAddress)
        {
            _dbContext.CustomerAddresses.Add(customerAddress);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCustomerAddress(CustomerAddress customerAddress)
        {
            _dbContext.CustomerAddresses.Update(customerAddress);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCustomerAddress(CustomerAddress customerAddress)
        {
            _dbContext.CustomerAddresses.Remove(customerAddress);
            await _dbContext.SaveChangesAsync();
        }
    }
}
