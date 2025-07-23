using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class UserMapper
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
                Permissions = staff?.Role?.Permissions == null ? null : [.. staff.Role.Permissions.Select(rp => rp.Permission!.Code)],
                CreatedBy = staff?.CreatedBy,
                CreatedByStaff = staff?.CreatedByStaff?.ToStaffDto(),
                IsActive = staff?.Account != null && staff.Account.IsActive,
            };
        }

        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Avatar = customer.Avatar,
                CreatedAt = customer.CreatedAt,
                IsActive = customer.Account != null && customer.Account.IsActive,
            };
        }
    }
}
