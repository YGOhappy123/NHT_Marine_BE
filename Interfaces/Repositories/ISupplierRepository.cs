using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ISupplierRepository
    {
        Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<Supplier>, int)> GetAllSuppliers(BaseQueryObject queryObject);
        Task<Supplier?> GetSupplierById(int supplierId);
        Task<Supplier?> GetSupplierByName(string supplierName);
        Task<bool> IsSSupplierBeingUsed(int supplierId);
        Task AddSupplier(Supplier supplier);
        Task UpdateSupplier(Supplier supplier);
        Task DeleteSupplier(Supplier supplier);
    }
}
