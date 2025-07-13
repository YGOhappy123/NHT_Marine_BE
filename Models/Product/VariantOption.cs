using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Product
{
    public class VariantOption
    {
        [Key]
        public int OptionId { get; set; }
        public int? VariantId { get; set; }
        public string Value { get; set; } = string.Empty;
        public ProductVariant? Variant { get; set; }
        public List<ProductAttribute> ProductItems { get; set; } = [];
    }
}
