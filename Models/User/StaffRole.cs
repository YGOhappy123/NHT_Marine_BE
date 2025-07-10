using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.User
{
    public class StaffRole
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsImmutable { get; set; } = false;
        public List<RolePermission> Permissions { get; set; } = [];
    }
}
