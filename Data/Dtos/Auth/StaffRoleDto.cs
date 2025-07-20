namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class StaffRoleDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsImmutable { get; set; } = false;
        public List<PermissionDto>? Permissions { get; set; }
    }

    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
