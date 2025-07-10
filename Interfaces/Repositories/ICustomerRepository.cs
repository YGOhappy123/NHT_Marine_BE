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
    }
}
