using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Product;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class ImportDto
    {
        public int ImportId { get; set; }
        public int? SupplierId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public DateTime ImportDate { get; set; }
        public bool IsDistributed { get; set; } = false;
        public DateTime TrackedAt { get; set; } = DateTime.Now;
        public int? TrackedBy { get; set; }
        public SupplierDto? Supplier { get; set; }
        public StaffDto? TrackedByStaff { get; set; }
        public List<ImportItemDto>? Items { get; set; } = [];
    }

    public class ImportItemDto
    {
        public int? ProductItemId { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public DetailedProductItemDto? ProductItem { get; set; }
    }
}
