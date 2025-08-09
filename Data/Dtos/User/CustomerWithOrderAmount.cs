using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Data.Dtos.User
{
    public class CustomerWithOrderAmount : Customer
    {
        public decimal OrderAmount { get; set; }

        public CustomerWithOrderAmount(Customer customer, decimal orderAmount)
        {
            this.CustomerId = customer.CustomerId;
            this.FullName = customer.FullName;
            this.Email = customer.Email;
            this.Avatar = customer.Avatar;
            this.CreatedAt = customer.CreatedAt;
            this.AccountId = customer.AccountId;
            this.Account = customer.Account;
            this.Orders = customer.Orders;
            this.OrderAmount = orderAmount;
        }
    }
}
