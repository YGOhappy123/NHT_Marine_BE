using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
        Task<ServiceResponse<List<StaffRole>>> GetAllRoles(BaseQueryObject queryObject);
    }
}
