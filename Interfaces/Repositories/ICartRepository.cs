using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<CustomerCart?> GetCustomerActiveCart(int customerId);
        Task ConvertActiveCart(int customerId);
        Task AddCustomerCart(CustomerCart cart);
        Task UpdateCustomerCart(CustomerCart cart);
        Task AddCartItem(CartItem cartItem);
        Task UpdateCartItem(CartItem cartItem);
        Task DeleteCartItem(CartItem cartItem);
    }
}
