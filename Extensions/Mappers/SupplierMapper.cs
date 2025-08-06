using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class SupplierMapper
    {
        public static SupplierDto ToSupplierDto(this Supplier supplier)
        {
            return new SupplierDto
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Address = supplier.Address,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
            };
        }
    }
}
