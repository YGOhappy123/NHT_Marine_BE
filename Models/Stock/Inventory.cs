using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Models.Stock
{
    public class Inventory
    {
        public int? StorageId { get; set; }
        public int? ProductItemId { get; set; }
        public int Quantity { get; set; }
        public Storage? Storage { get; set; }
        public ProductItem? ProductItem { get; set; }
    }
}
