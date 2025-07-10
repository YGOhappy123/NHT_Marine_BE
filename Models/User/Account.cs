using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.User
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Customer? Customer { get; set; }
        public Staff? Staff { get; set; }
    }
}
