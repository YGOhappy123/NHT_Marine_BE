using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.User;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<ServiceResponse> UpdateCustomerProfile(UpdateUserDto updateDto, int authUserId);
        Task<ServiceResponse<CustomerCart?>> GetCustomerCart(int authUserId);
        Task<ServiceResponse> AddCartItem(AddCartItemDto addDto, int authUserId);
        Task<ServiceResponse> UpdateCartItem(UpdateCartItemDto updateDto, int productItemId, int authUserId);
        Task<ServiceResponse> RemoveCartItem(int productItemId, int authUserId);
        Task<ServiceResponse> ResetCustomerCart(int authUserId);
        Task<ServiceResponse<List<Customer>>> GetAllCustomers(BaseQueryObject queryObject);
        Task<ServiceResponse> DeactivateCustomerAccount(int targetCustomerId, int authRoleId);
    }
}
