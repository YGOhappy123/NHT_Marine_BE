namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class ProductItemDto
    {
        public int ProductItemId { get; set; }
        public int? RootProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PackingGuide { get; set; } = string.Empty;
        public RootProductDto? RootProduct { get; set; }
        public List<ProductAttributeDto>? Attributes { get; set; } = [];
        public int? Stock { get; set; }
    }

    public class ProductAttributeDto
    {
        public int? OptionId { get; set; }
    }
}
