using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.User;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IStaffService
    {
        Task<ServiceResponse<List<Staff>>> GetAllStaffs(BaseQueryObject queryObject);
        Task<ServiceResponse> CreateNewStaff(CreateStaffDto createStaffDto, int authUserId, int authRoleId);
        Task<ServiceResponse> UpdateStaffProfile(UpdateUserDto updateDto, int targetStaffId, int authUserId, int authRoleId);
        Task<ServiceResponse> ChangeStaffRole(ChangeStaffRoleDto changeStaffRoleDto, int targetStaffId, int authRoleId);
        Task<ServiceResponse> StaffDeactivateAccount(int targetStaffId, int authUserId, int authRoleId);
    }
}
