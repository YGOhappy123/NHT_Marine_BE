using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public AccountRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Account?> GetAccountByUsername(string username)
        {
            return await _dbContext.Accounts.SingleOrDefaultAsync(acc => acc.IsActive && acc.Username == username);
        }

        public async Task<Account?> GetAccountById(int accountId)
        {
            return await _dbContext.Accounts.SingleOrDefaultAsync(acc => acc.IsActive && acc.AccountId == accountId);
        }

        public async Task<Account?> GetCustomerAccountByEmail(string email)
        {
            return await _dbContext
                .Accounts.Where(acc => acc.IsActive && acc.Customer != null && acc.Customer.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountByUserId(int userId, bool isCustomer)
        {
            if (isCustomer)
            {
                return await _dbContext
                    .Accounts.Where(acc => acc.IsActive && acc.Customer != null && acc.Customer.CustomerId == userId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return await _dbContext
                    .Accounts.Where(acc => acc.IsActive && acc.Staff != null && acc.Staff.StaffId == userId)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task AddAccount(Account account)
        {
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAccount(Account account)
        {
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }
    }
}
