using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepo;

        public RoleService(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _roleRepo.VerifyPermission(authRoleId, permission);
            if (isVerified)
            {
                return new ServiceResponse<bool>
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.VERIFY_PERMISSION_SUCCESSFULLY,
                };
            }
            else
            {
                return new ServiceResponse<bool>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }
        }

        public async Task<ServiceResponse<List<StaffRole>>> GetAllRoles(BaseQueryObject queryObject)
        {
            var (roles, total) = await _roleRepo.GetAllRoles(queryObject);

            return new ServiceResponse<List<StaffRole>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = roles,
                Total = total,
                Took = roles.Count,
            };
        }

        public async Task<ServiceResponse<StaffRole?>> GetRoleById(int roleId, int authRoleId)
        {
            var hasViewRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ACCESS_ROLE_DASHBOARD_PAGE.ToString());
            if (!hasViewRolePermission && roleId != authRoleId)
            {
                return new ServiceResponse<StaffRole?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var role = await _roleRepo.GetRoleById(roleId);
            return new ServiceResponse<StaffRole?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = role,
            };
        }
    }
}
