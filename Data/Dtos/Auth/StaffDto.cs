namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class StaffDto
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? RoleId { get; set; }
        public string? Role { get; set; }
        public string[]? Permissions { get; set; }
        public int? CreatedBy { get; set; }
        public StaffDto? CreatedByStaff { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
