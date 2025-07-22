using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class AppPermissionMapper
    {
        public static AppPermissionDto ToAppPermissionDto(this AppPermission permission)
        {
            return new AppPermissionDto
            {
                PermissionId = permission.PermissionId,
                Name = permission.Name,
                Code = permission.Code,
            };
        }
    }
}
