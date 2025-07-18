using NHT_Marine_BE.Enums;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        public Task<bool> VerifyPermission(int authRoleId, string permission);
    }
}
