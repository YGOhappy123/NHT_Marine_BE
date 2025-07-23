using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<StaffRole>, int)> GetAllRoles(BaseQueryObject queryObject);
        Task<StaffRole?> GetRoleById(int roleId);
        Task<StaffRole?> GetRoleByName(string roleName);
        Task<bool> IsRoleBeingUsed(int roleId);
        Task AddRole(StaffRole role);
        Task UpdateRole(StaffRole role);
        Task DeleteRole(StaffRole role);
    }
}
