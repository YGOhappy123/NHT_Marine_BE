using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Data.Validators;

namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class CreateProductDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<AddProductVariantDto> Variants { get; set; } = [];

        [Required]
        [MinLength(1)]
        public List<AddProductItemDto> ProductItems { get; set; } = [];
    }

    public class AddProductVariantDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public bool IsAdjustable { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Options { get; set; } = [];
    }

    public class AddProductItemDto
    {
        [Required]
        [DivisibleBy(1000)]
        public int Price { get; set; }

        [Required]
        public string PackingGuide { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<int> Attributes { get; set; } = [];
    }
}
