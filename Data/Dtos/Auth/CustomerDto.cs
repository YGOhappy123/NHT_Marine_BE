namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsActive { get; set; } = true;
    }
}
