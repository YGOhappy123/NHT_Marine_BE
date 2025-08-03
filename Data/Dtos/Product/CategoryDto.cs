using NHT_Marine_BE.Data.Dtos.Auth;

namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public int? ParentId { get; set; }
        public StaffDto? CreatedByStaff { get; set; }
        public CategoryDto? ParentCategory { get; set; }
    }
}
