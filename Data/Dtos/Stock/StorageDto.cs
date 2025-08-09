namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class StorageDto
    {
        public int StorageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? TypeId { get; set; }
        public StorageTypeDto? Type { get; set; }
    }
}
