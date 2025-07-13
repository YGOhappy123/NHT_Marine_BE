using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Stock
{
    public class Storage
    {
        [Key]
        public int StorageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? TypeId { get; set; }
        public StorageType? Type { get; set; }
        public List<Inventory> ProductItems { get; set; } = [];
    }
}
