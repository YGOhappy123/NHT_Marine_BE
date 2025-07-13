using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Stock
{
    public class ProductImport
    {
        [Key]
        public int ImportId { get; set; }
        public int? SupplierId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime TrackedAt { get; set; } = DateTime.Now;
        public int? TrackedBy { get; set; }
        public Supplier? Supplier { get; set; }
        public Staff? TrackedByStaff { get; set; }
        public List<ImportItem> Items { get; set; } = [];
    }
}
