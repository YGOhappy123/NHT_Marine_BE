using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Models.User
{
    public class Customer : AppUser
    {
        [Key]
        public int CustomerId { get; set; }
        public List<CustomerAddress> Addresses { get; set; } = [];
        public List<CustomerCart> Carts { get; set; } = [];
        public List<Conversation> Conversations { get; set; } = [];
    }
}
