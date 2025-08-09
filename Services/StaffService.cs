using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.User;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class StaffService : IStaffService
    {
        private readonly IConfiguration _configuration;
        private readonly IMailerService _mailerService;
        private readonly IAccountRepository _accountRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IRoleRepository _roleRepo;

        public StaffService(
            IConfiguration configuration,
            IMailerService mailerService,
            IAccountRepository accountRepo,
            IStaffRepository staffRepo,
            IRoleRepository roleRepo
        )
        {
            _configuration = configuration;
            _mailerService = mailerService;
            _accountRepo = accountRepo;
            _staffRepo = staffRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<List<Staff>>> GetAllStaffs(BaseQueryObject queryObject)
        {
            var (staffs, total) = await _staffRepo.GetAllStaffs(queryObject);

            return new ServiceResponse<List<Staff>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = staffs,
                Total = total,
                Took = staffs.Count,
            };
        }

        public async Task<ServiceResponse> CreateNewStaff(CreateStaffDto createStaffDto, int authUserId, int authRoleId)
        {
            var hasCreateStaffPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_STAFF.ToString());
            if (!hasCreateStaffPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var staffWithThisEmail = await _staffRepo.GetStaffByEmail(createStaffDto.Email);
            if (staffWithThisEmail != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.EMAIL_EXISTED,
                };
            }

            var role = await _roleRepo.GetRoleById(createStaffDto.RoleId);
            if (role == null || role.IsImmutable)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.INVALID_ROLE_SELECTED,
                };
            }

            string randomUsername = RandomStringGenerator.GenerateRandomString(16);
            string randomPassword = RandomStringGenerator.GenerateRandomString(16);

            var newAccount = new Account { Username = randomUsername, Password = BCrypt.Net.BCrypt.HashPassword(randomPassword) };
            await _accountRepo.AddAccount(newAccount);

            var newStaff = new Staff
            {
                AccountId = newAccount.AccountId,
                FullName = createStaffDto.FullName,
                Email = createStaffDto.Email,
                Avatar = createStaffDto.Avatar,
                RoleId = createStaffDto.RoleId,
                CreatedBy = authUserId,
            };
            await _staffRepo.AddStaff(newStaff);

            await _mailerService.SendWelcomeNewStaffEmail(
                createStaffDto.Email,
                createStaffDto.FullName,
                randomUsername,
                randomPassword,
                $"{_configuration["Application:DashboardUrl"]}/change-password"
            );

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.CREATE_STAFF_SUCCESSFULLY,
            };
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

        public async Task<ServiceResponse> ChangeStaffRole(ChangeStaffRoleDto changeStaffRoleDto, int targetStaffId, int authRoleId)
        {
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

            var hasChangeRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.CHANGE_STAFF_ROLE.ToString());
            if (!hasChangeRolePermission || targetStaff.Role!.IsImmutable)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var role = await _roleRepo.GetRoleById(changeStaffRoleDto.RoleId);
            if (role == null || role.IsImmutable)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.INVALID_ROLE_SELECTED,
                };
            }

            targetStaff.RoleId = changeStaffRoleDto.RoleId;
            await _staffRepo.UpdateStaff(targetStaff);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_USER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> StaffDeactivateAccount(int targetStaffId, int authUserId, int authRoleId)
        {
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

            var hasDeactivateStaffPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DEACTIVATE_STAFF_ACCOUNT.ToString());
            if (!hasDeactivateStaffPermission || targetStaff.Role!.IsImmutable || targetStaffId == authUserId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            targetStaff.Account!.IsActive = false;
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
