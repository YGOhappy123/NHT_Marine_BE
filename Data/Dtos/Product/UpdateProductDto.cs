using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Data.Validators;

namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class UpdateProductInfoDto
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
    }

    public class UpdateProductItemsDto
    {
        [Required]
        [MinLength(1)]
        public List<UpdateProductItemDto> ProductItems { get; set; } = [];
    }

    public class UpdateProductItemDto
    {
        [Required]
        public int ProductItemId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [DivisibleBy(1000)]
        public int Price { get; set; }

        [Required]
        public string PackingGuide { get; set; } = string.Empty;
    }
}
