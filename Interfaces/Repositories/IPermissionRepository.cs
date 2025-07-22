using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<(List<AppPermission>, int)> GetAllPermissions(BaseQueryObject queryObject);
    }
}
