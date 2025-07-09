using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
