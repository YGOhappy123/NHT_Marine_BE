using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Data.Validators;

namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }
}
