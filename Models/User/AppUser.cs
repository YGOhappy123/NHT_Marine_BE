using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Models.User
{
    public class AppUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
