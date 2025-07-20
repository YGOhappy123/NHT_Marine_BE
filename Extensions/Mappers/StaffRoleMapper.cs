using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class StaffRoleMapper
    {
        public static StaffRoleDto ToStaffRoleDto(this StaffRole role)
        {
            return new StaffRoleDto
            {
                RoleId = role.RoleId,
                Name = role.Name,
                IsImmutable = role.IsImmutable,
                Permissions = role
                    .Permissions?.Select(rp => new PermissionDto
                    {
                        PermissionId = rp!.Permission!.PermissionId,
                        Name = rp!.Permission!.Name,
                        Code = rp!.Permission!.Code,
                    })
                    .ToList(),
            };
        }
    }
}
