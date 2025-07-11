using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByUsername(string username);
        Task<Account?> GetAccountById(int accountId);
        Task<Account?> GetCustomerAccountByEmail(string email);
        Task<Account?> GetAccountByUserId(int userId, bool isCustomer);
        Task AddAccount(Account account);
        Task UpdateAccount(Account account);
    }
}
