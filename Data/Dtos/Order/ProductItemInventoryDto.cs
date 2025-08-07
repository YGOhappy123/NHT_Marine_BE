using NHT_Marine_BE.Data.Dtos.Stock;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class ProductInventoryDto
    {
        public int ProductItemId { get; set; }
        public List<ProductItemStorageDto> Storages { get; set; } = [];
    }

    public class ProductItemStorageDto
    {
        public int StorageId { get; set; }
        public int Quantity { get; set; }
        public StorageDto? Storage { get; set; }
    }
}
