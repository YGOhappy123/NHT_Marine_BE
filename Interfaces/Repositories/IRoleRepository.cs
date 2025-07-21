using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        public Task<bool> VerifyPermission(int authRoleId, string permission);
        Task<(List<StaffRole>, int)> GetAllRoles(BaseQueryObject queryObject);
        Task<StaffRole?> GetRoleById(int roleId);
    }
}
