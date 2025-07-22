using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<StaffRole>>> GetAllRoles(BaseQueryObject queryObject);
        Task<ServiceResponse<StaffRole?>> GetRoleById(int roleId, int authRoleId);
        Task<ServiceResponse> AddNewRole(CreateUpdateRoleDto createDto, int authRoleId);
        Task<ServiceResponse> UpdateRole(CreateUpdateRoleDto updateDto, int targetRoleId, int authRoleId);
        Task<ServiceResponse> RemoveRole(int targetRoleId, int authRoleId);
        Task<ServiceResponse<List<AppPermission>>> GetAllPermissions(BaseQueryObject queryObject);
    }
}
