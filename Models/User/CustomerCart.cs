using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Enums;

namespace NHT_Marine_BE.Models.User
{
    public class CustomerCart
    {
        [Key]
        public int CartId { get; set; }
        public int? CustomerId { get; set; }
        public CartStatus Status { get; set; } = CartStatus.Active;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Customer? Customer { get; set; }
        public List<CartItem> Items { get; set; } = [];
    }
}
