using NHT_Marine_BE.Data.Dtos.User;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerById(int customerId);
        Task<Customer?> GetCustomerByAccountId(int accountId);
        Task<Customer?> GetCustomerByEmail(string email);
        Task<(List<Customer>, int)> GetAllCustomers(BaseQueryObject queryObject);
        Task AddCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
        Task<List<Customer>> GetAllCustomersInTimeRange(DateTime startTime, DateTime endTime);
        Task<List<CustomerWithOrderCount>> GetCustomersWithHighestOrderCountInTimeRange(DateTime startTime, DateTime endTime, int limit);
        Task<List<CustomerWithOrderAmount>> GetCustomersWithHighestOrderAmountInTimeRange(DateTime startTime, DateTime endTime, int limit);
    }
}
