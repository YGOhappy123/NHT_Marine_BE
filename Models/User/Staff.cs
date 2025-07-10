using System.ComponentModel.DataAnnotations;

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
        public List<Staff> CreatedStaffs { get; set; } = [];
    }
}
