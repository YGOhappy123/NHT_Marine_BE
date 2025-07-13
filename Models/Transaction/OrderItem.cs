using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Models.Transaction
{
    public class OrderItem
    {
        public int? OrderId { get; set; }
        public int? ProductItemId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Order? Order { get; set; }
        public ProductItem? ProductItem { get; set; }
    }
}
