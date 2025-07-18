using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Enums;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission);
    }
}
