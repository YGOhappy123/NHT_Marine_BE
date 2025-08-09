using NHT_Marine_BE.Data.Dtos.Product;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class StorageDto
    {
        public int StorageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? TypeId { get; set; }
        public StorageTypeDto? Type { get; set; }
        public List<InventoryDto>? ProductItems { get; set; } = [];
    }

    public class InventoryDto
    {
        public int Quantity { get; set; }
        public int? ProductItemId { get; set; }
        public DetailedProductItemDto? ProductItem { get; set; }
    }
}
