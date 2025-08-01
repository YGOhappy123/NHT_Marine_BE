using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CartRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<CustomerCart?> GetCustomerActiveCart(int customerId)
        {
            return await _dbContext
                .CustomerCarts.Include(cc => cc.Items)
                .Where(cc => cc.CustomerId == customerId && cc.Status == CartStatus.Active)
                .FirstOrDefaultAsync();
        }

        public async Task AddCustomerCart(CustomerCart cart)
        {
            _dbContext.CustomerCarts.Add(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCustomerCart(CustomerCart cart)
        {
            _dbContext.CustomerCarts.Update(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItemByProductItemId(int productItemId)
        {
            return await _dbContext
                .CartItems.Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Cart != null && ci.Cart.Status == CartStatus.Active && ci.ProductItemId == productItemId);
        }

        public async Task AddCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Add(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Update(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
        }
    }
}
