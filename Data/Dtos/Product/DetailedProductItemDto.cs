namespace NHT_Marine_BE.Data.Dtos.Product
{
    public class DetailedProductItemDto
    {
        public int ProductItemId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<PartialAttributeDto> Attributes { get; set; } = [];
        public PartialRootProductDto? RootProduct { get; set; }
        public int Stock { get; set; }
        public decimal DiscountRate { get; set; }
    }

    public class PartialAttributeDto
    {
        public string Variant { get; set; } = string.Empty;
        public string Option { get; set; } = string.Empty;
    }

    public class PartialRootProductDto
    {
        public int RootProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
