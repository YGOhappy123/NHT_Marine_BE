using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Product
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public int? ParentId { get; set; }
        public Staff? CreatedByStaff { get; set; }
        public Category? ParentCategory { get; set; }
        public List<Category> ChildrenCategories { get; set; } = [];
    }
}
