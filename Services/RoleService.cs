using NHT_Marine_BE.Data.Dtos.Auth;
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
        private readonly IPermissionRepository _permissionRepo;

        public RoleService(IRoleRepository roleRepo, IPermissionRepository permissionRepo)
        {
            _roleRepo = roleRepo;
            _permissionRepo = permissionRepo;
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

        public async Task<ServiceResponse> AddNewRole(CreateUpdateRoleDto createDto, int authRoleId)
        {
            var hasAddRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_ROLE.ToString());
            if (!hasAddRolePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var roleWithSameName = await _roleRepo.GetRoleByName(createDto.Name);
            if (roleWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.ROLE_EXISTED,
                };
            }

            var newRole = new StaffRole
            {
                Name = createDto.Name.CapitalizeAllWords(),
                IsImmutable = false,
                Permissions = [],
            };

            foreach (var permission in createDto.Permissions.Distinct())
            {
                newRole.Permissions.Add(new RolePermission { PermissionId = permission });
            }

            await _roleRepo.AddRole(newRole);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_ROLE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateRole(CreateUpdateRoleDto updateDto, int targetRoleId, int authRoleId)
        {
            var hasUpdateRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_ROLE.ToString());
            if (!hasUpdateRolePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetRole = await _roleRepo.GetRoleById(targetRoleId);
            if (targetRole == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ROLE_NOT_FOUND,
                };
            }

            var roleWithSameName = await _roleRepo.GetRoleByName(updateDto.Name);
            if (roleWithSameName != null && roleWithSameName.RoleId != targetRoleId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.ROLE_EXISTED,
                };
            }
            targetRole.Name = updateDto.Name.CapitalizeAllWords();

            var currentPermissions = targetRole.Permissions.Select(rp => (int)rp.PermissionId!).ToList();
            var newPermissions = updateDto.Permissions.Distinct().ToList();

            var toAdd = newPermissions.Except(currentPermissions);
            foreach (var permissionId in toAdd)
            {
                targetRole.Permissions.Add(new RolePermission { PermissionId = permissionId });
            }

            var toRemove = currentPermissions.Except(newPermissions);
            targetRole.Permissions.RemoveAll(p => toRemove.Contains(p.PermissionId!.Value));

            await _roleRepo.UpdateRole(targetRole);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_ROLE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveRole(int targetRoleId, int authRoleId)
        {
            var hasRemoveRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.REMOVE_ROLE.ToString());
            if (!hasRemoveRolePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetRole = await _roleRepo.GetRoleById(targetRoleId);
            if (targetRole == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ROLE_NOT_FOUND,
                };
            }

            var isRoleBeingUsed = await _roleRepo.IsRoleBeingUsed(targetRoleId);
            if (isRoleBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ROLE_BEING_USED,
                };
            }

            await _roleRepo.DeleteRole(targetRole);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_ROLE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<AppPermission>>> GetAllPermissions(BaseQueryObject queryObject)
        {
            var (permissions, total) = await _permissionRepo.GetAllPermissions(queryObject);

            return new ServiceResponse<List<AppPermission>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = permissions,
                Total = total,
                Took = permissions.Count,
            };
        }
    }
}
