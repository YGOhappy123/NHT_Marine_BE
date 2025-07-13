using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Models.User
{
    public class Customer : AppUser
    {
        [Key]
        public int CustomerId { get; set; }
        public List<CustomerAddress> Addresses { get; set; } = [];
        public List<CustomerCart> Carts { get; set; } = [];
        public List<Conversation> Conversations { get; set; } = [];
        public List<Order> Orders { get; set; } = [];
    }
}
