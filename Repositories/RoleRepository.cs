using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;

namespace NHT_Marine_BE.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public RoleRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> VerifyPermission(int authRoleId, string permission)
        {
            return await _dbContext.RolesPermissions.AnyAsync(rp =>
                rp.RoleId == authRoleId && rp.Permission != null && rp.Permission.Code == permission
            );
        }
    }
}
