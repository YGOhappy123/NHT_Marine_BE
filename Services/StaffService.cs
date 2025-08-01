using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.User;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IRoleRepository _roleRepo;

        public StaffService(IStaffRepository staffRepo, IRoleRepository roleRepo)
        {
            _staffRepo = staffRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse> UpdateStaffProfile(UpdateUserDto updateDto, int targetStaffId, int authUserId, int authRoleId)
        {
            var hasUpdateStaffPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_STAFF_INFORMATION.ToString());
            var hasUpdatePersonalPermission =
                targetStaffId == authUserId
                && await _roleRepo.VerifyPermission(authRoleId, Permission.MODIFY_PERSONAL_INFORMATION.ToString());
            if (!hasUpdateStaffPermission && !hasUpdatePersonalPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetStaff = await _staffRepo.GetStaffById(targetStaffId);
            if (targetStaff == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            targetStaff.FullName = updateDto.FullName;
            targetStaff.Avatar = updateDto.Avatar;

            var staffWithThisEmail = await _staffRepo.GetStaffByEmail(updateDto.Email);
            if (staffWithThisEmail != null && staffWithThisEmail.StaffId != targetStaffId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.EMAIL_EXISTED,
                };
            }

            targetStaff.Email = updateDto.Email;

            await _staffRepo.UpdateStaff(targetStaff);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_USER_SUCCESSFULLY,
            };
        }
    }
}
