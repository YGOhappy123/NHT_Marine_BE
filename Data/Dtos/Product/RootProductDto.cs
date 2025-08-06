using NHT_Marine_BE.Data.Dtos.Auth;

namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class RootProductDto
    {
        public int RootProductId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public CategoryDto? Category { get; set; }
        public StaffDto? CreatedByStaff { get; set; }
        public List<ProductVariantDto>? Variants { get; set; } = [];
        public List<ProductItemDto>? ProductItems { get; set; } = [];
        public List<PromotionDto>? Promotions { get; set; } = [];
        public decimal? DiscountRate { get; set; }
    }

    public class ProductVariantDto
    {
        public int VariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsAdjustable { get; set; } = false;
        public List<VariantOptionDto>? Options { get; set; } = [];
    }

    public class VariantOptionDto
    {
        public int OptionId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
