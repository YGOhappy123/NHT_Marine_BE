using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Stock
{
    public class ProductDamageReport
    {
        [Key]
        public int ReportId { get; set; }
        public int? ProductItemId { get; set; }
        public int? StorageId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpectedCost { get; set; }
        public string? Note { get; set; }
        public int? TypeId { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.Now;
        public int? ReportedBy { get; set; }
        public ProductItem? ProductItem { get; set; }
        public Storage? Storage { get; set; }
        public DamageType? Type { get; set; }
        public Staff? ReportedByStaff { get; set; }
    }
}
