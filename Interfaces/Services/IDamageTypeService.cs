using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IDamageTypeService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<DamageType>>> GetAllDamageTypes(BaseQueryObject queryObject);
        Task<ServiceResponse<DamageType?>> GetDamageTypeById(int damageTypeId, int authRoleId);
        Task<ServiceResponse> AddNewDamageType(CreateUpdateDamageTypeDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateDamageType(CreateUpdateDamageTypeDto updateDto, int targetDamageTypeId, int authRoleId);
        Task<ServiceResponse> RemoveDamageType(int targetDamageTypeId, int authRoleId);
    }
}
