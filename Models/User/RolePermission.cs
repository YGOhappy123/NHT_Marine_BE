namespace NHT_Marine_BE.Models.User
{
    public class RolePermission
    {
        public int? RoleId { get; set; }
        public int? PermissionId { get; set; }
        public StaffRole? Role { get; set; }
        public AppPermission? Permission { get; set; }
    }
}
