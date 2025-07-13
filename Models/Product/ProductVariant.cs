using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Product
{
    public class ProductVariant
    {
        [Key]
        public int VariantId { get; set; }
        public int? RootProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsAdjustable { get; set; } = false;
        public RootProduct? RootProduct { get; set; }
        public List<VariantOption> Options { get; set; } = [];
    }
}
