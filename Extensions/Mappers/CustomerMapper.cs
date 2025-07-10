using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Avatar = customer.Avatar,
                CreatedAt = customer.CreatedAt,
                IsActive = customer.Account != null && customer.Account.IsActive,
            };
        }
    }
}
