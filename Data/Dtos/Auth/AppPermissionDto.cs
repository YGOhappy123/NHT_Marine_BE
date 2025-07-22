namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class AppPermissionDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
