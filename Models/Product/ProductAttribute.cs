namespace NHT_Marine_BE.Models.Product
{
    public class ProductAttribute
    {
        public int? ProductItemId { get; set; }
        public int? OptionId { get; set; }
        public ProductItem? ProductItem { get; set; }
        public VariantOption? Option { get; set; }
    }
}
