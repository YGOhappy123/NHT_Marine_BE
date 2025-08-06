using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface ISupplierService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<Supplier>>> GetAllSuppliers(BaseQueryObject queryObject);
        Task<ServiceResponse<Supplier?>> GetSupplierById(int supplierId, int authRoleId);
        Task<ServiceResponse> AddNewSupplier(CreateUpdateSupplierDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateSupplier(CreateUpdateSupplierDto updateDto, int targetSupplierId, int authRoleId);
        Task<ServiceResponse> RemoveSupplier(int targetSupplierId, int authRoleId);
    }
}
