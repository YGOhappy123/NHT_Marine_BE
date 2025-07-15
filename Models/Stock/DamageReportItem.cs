using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Models.Stock
{
    public class DamageReportItem
    {
        public int? ReportId { get; set; }
        public int? ProductItemId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpectedCost { get; set; }
        public int Quantity { get; set; }
        public ProductDamageReport? Report { get; set; }
        public ProductItem? ProductItem { get; set; }
    }
}
