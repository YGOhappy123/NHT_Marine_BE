using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Product
{
    public class ProductItem
    {
        [Key]
        public int ProductItemId { get; set; }
        public int? RootProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string PackingGuide { get; set; } = string.Empty;
        public RootProduct? RootProduct { get; set; }
        public List<CartItem> CartItems { get; set; } = [];
        public List<ProductAttribute> Attributes { get; set; } = [];
        public List<OrderItem> Orders { get; set; } = [];
        public List<Inventory> Storages { get; set; } = [];
        public List<ImportItem> Imports { get; set; } = [];
    }
}
