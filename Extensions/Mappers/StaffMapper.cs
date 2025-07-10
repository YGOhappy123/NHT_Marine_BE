using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class StaffMapper
    {
        public static StaffDto ToStaffDto(this Staff staff)
        {
            return new StaffDto
            {
                StaffId = staff.StaffId,
                FullName = staff.FullName,
                Email = staff.Email,
                Avatar = staff.Avatar,
                CreatedAt = staff.CreatedAt,
                RoleId = staff.RoleId,
                Role = staff.Role?.Name,
                CreatedBy = staff.CreatedBy,
                CreatedByStaff = staff.CreatedByStaff?.FullName,
                IsActive = staff.Account != null && staff.Account.IsActive,
            };
        }
    }
}
