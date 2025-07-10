using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.User
{
    public class AppPermission
    {
        [Key]
        public int PermissionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public List<RolePermission> Roles { get; set; } = [];
    }
}
