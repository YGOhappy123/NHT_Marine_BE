using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class ProductImportMapper
    {
        public static ImportDto ToImportDto(this ProductImport import)
        {
            return new ImportDto
            {
                ImportId = import.ImportId,
                SupplierId = import.SupplierId,
                InvoiceNumber = import.InvoiceNumber,
                TotalCost = import.TotalCost,
                ImportDate = import.ImportDate,
                IsDistributed = import.IsDistributed,
                TrackedAt = import.TrackedAt,
                TrackedBy = import.TrackedBy,
                Supplier = import.Supplier?.ToSupplierDto(),
                TrackedByStaff = import.TrackedByStaff?.ToStaffDto(),
                Items = import
                    .Items?.Select(ii => new ImportItemDto
                    {
                        ProductItemId = ii.ProductItemId,
                        Cost = ii.Cost,
                        Quantity = ii.Quantity,
                    })
                    .ToList(),
            };
        }
    }
}
