using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICustomerAddressRepository
    {
        Task<(List<CustomerAddress>, int)> GetCustomerAddresses(BaseQueryObject queryObject, int customerId);
        Task<CustomerAddress?> GetCustomerAddressById(int addressId);
        Task<CustomerAddress?> GetCustomerAddressExactMatch(
            string recipientName,
            string phoneNumber,
            string city,
            string ward,
            string addressLine,
            int customerId
        );
        Task<CustomerAddress?> GetCustomerDefaultAddress(int customerId);
        Task AddCustomerAddress(CustomerAddress customerAddress);
        Task UpdateCustomerAddress(CustomerAddress customerAddress);
        Task DeleteCustomerAddress(CustomerAddress customerAddress);
    }
}
