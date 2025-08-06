using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class DamageTypeService : IDamageTypeService
    {
        private readonly IDamageTypeRepository _damageTypeRepo;
        private readonly IRoleRepository _roleRepo;

        public DamageTypeService(IDamageTypeRepository damageTypeRepo, IRoleRepository roleRepo)
        {
            _damageTypeRepo = damageTypeRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _damageTypeRepo.VerifyPermission(authRoleId, permission);
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

        public async Task<ServiceResponse<List<DamageType>>> GetAllDamageTypes(BaseQueryObject queryObject)
        {
            var (damageTypes, total) = await _damageTypeRepo.GetAllDamageTypes(queryObject);

            return new ServiceResponse<List<DamageType>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = damageTypes,
                Total = total,
                Took = damageTypes.Count,
            };
        }

        public async Task<ServiceResponse<DamageType?>> GetDamageTypeById(int damageTypeId, int authRoleId)
        {
            var hasViewDamageTypePermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ACCESS_DAMAGE_REPORT_DASHBOARD_PAGE.ToString()
            );
            if (!hasViewDamageTypePermission && damageTypeId != authRoleId)
            {
                return new ServiceResponse<DamageType?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var damageType = await _damageTypeRepo.GetDamageTypeById(damageTypeId);
            return new ServiceResponse<DamageType?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = damageType,
            };
        }

        public async Task<ServiceResponse> AddNewDamageType(CreateUpdateDamageTypeDto createDto, int authRoleId)
        {
            var hasAddDamageTypePermission = await _damageTypeRepo.VerifyPermission(
                authRoleId,
                Permission.ADD_NEW_DAMAGE_CATEGORY.ToString()
            );
            if (!hasAddDamageTypePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var damageTypeWithSameName = await _damageTypeRepo.GetDamageTypeByName(createDto.Name);
            if (damageTypeWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DAMAGE_TYPE_EXISTED,
                };
            }

            var newDamageType = new DamageType { Name = createDto.Name.CapitalizeAllWords() };

            await _damageTypeRepo.AddDamageType(newDamageType);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_DAMAGE_TYPE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateDamageType(CreateUpdateDamageTypeDto updateDto, int targetDamageTypeId, int authRoleId)
        {
            var hasUpdateDamageTypePermission = await _damageTypeRepo.VerifyPermission(
                authRoleId,
                Permission.UPDATE_DAMAGE_CATEGORY.ToString()
            );
            if (!hasUpdateDamageTypePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetDamageType = await _damageTypeRepo.GetDamageTypeById(targetDamageTypeId);
            if (targetDamageType == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.DAMAGE_TYPE_NOT_FOUND,
                };
            }

            var damageTypeWithSameName = await _damageTypeRepo.GetDamageTypeByName(updateDto.Name);
            if (damageTypeWithSameName != null && damageTypeWithSameName.TypeId != targetDamageTypeId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DAMAGE_TYPE_EXISTED,
                };
            }
            targetDamageType.Name = updateDto.Name.CapitalizeAllWords();

            await _damageTypeRepo.UpdateDamageType(targetDamageType);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_DAMAGE_TYPE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveDamageType(int targetDamageTypeId, int authRoleId)
        {
            var hasRemoveDamageTypePermission = await _damageTypeRepo.VerifyPermission(
                authRoleId,
                Permission.DELETE_DAMAGE_CATEGORY.ToString()
            );
            if (!hasRemoveDamageTypePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetDamageType = await _damageTypeRepo.GetDamageTypeById(targetDamageTypeId);
            if (targetDamageType == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.DAMAGE_TYPE_NOT_FOUND,
                };
            }

            var isDamageTypeBeingUsed = await _damageTypeRepo.IsDamageTypeBeingUsed(targetDamageTypeId);
            if (isDamageTypeBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.DAMAGE_TYPE_BEING_USED,
                };
            }

            await _damageTypeRepo.DeleteDamageType(targetDamageType);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_DAMAGE_TYPE_SUCCESSFULLY,
            };
        }
    }
}
