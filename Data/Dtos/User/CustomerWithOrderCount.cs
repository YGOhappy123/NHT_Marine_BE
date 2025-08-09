using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Data.Dtos.User
{
    public class CustomerWithOrderCount : Customer
    {
        public int OrderCount { get; set; }

        public CustomerWithOrderCount(Customer customer, int orderCount)
        {
            this.CustomerId = customer.CustomerId;
            this.FullName = customer.FullName;
            this.Email = customer.Email;
            this.Avatar = customer.Avatar;
            this.CreatedAt = customer.CreatedAt;
            this.AccountId = customer.AccountId;
            this.Account = customer.Account;
            this.Orders = customer.Orders;
            this.OrderCount = orderCount;
        }
    }
}
