namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class AcceptOrderDto
    {
        public int StatusId { get; set; }
        public List<ChooseInventoryDto> Inventories { get; set; } = [];
    }

    public class ChooseInventoryDto
    {
        public int ProductItemId { get; set; }
        public List<ChooseStorageDto> Storages { get; set; } = [];
    }

    public class ChooseStorageDto
    {
        public int StorageId { get; set; }
        public int Quantity { get; set; }
    }
}
