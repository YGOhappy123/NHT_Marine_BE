using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Models.User
{
    public class Staff : AppUser
    {
        [Key]
        public int StaffId { get; set; }
        public int? RoleId { get; set; }
        public int? CreatedBy { get; set; }
        public StaffRole? Role { get; set; }
        public Staff? CreatedByStaff { get; set; }
    }
}
