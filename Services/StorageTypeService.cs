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
    public class StorageTypeService : IStorageTypeService
    {
        private readonly IStorageTypeRepository _storageTypeRepo;
        private readonly IRoleRepository _roleRepo;

        public StorageTypeService(IStorageTypeRepository storageTypeRepo, IRoleRepository roleRepo)
        {
            _storageTypeRepo = storageTypeRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _storageTypeRepo.VerifyPermission(authRoleId, permission);
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

        public async Task<ServiceResponse<List<StorageType>>> GetAllStorageTypes(BaseQueryObject queryObject)
        {
            var (storageTypes, total) = await _storageTypeRepo.GetAllStorageTypes(queryObject);

            return new ServiceResponse<List<StorageType>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = storageTypes,
                Total = total,
                Took = storageTypes.Count,
            };
        }

        public async Task<ServiceResponse<StorageType?>> GetStorageTypeById(int storageTypeId, int authRoleId)
        {
            var hasViewStorageTypePermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ACCESS_STORAGE_DASHBOARD_PAGE.ToString()
            );
            if (!hasViewStorageTypePermission && storageTypeId != authRoleId)
            {
                return new ServiceResponse<StorageType?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var storageType = await _storageTypeRepo.GetStorageTypeById(storageTypeId);
            return new ServiceResponse<StorageType?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = storageType,
            };
        }

        public async Task<ServiceResponse> AddNewStorageType(CreateUpdateStorageTypeDto createDto, int authRoleId)
        {
            var hasAddStorageTypePermission = await _storageTypeRepo.VerifyPermission(
                authRoleId,
                Permission.ADD_NEW_STORAGE_TYPE.ToString()
            );
            if (!hasAddStorageTypePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var storageTypeWithSameName = await _storageTypeRepo.GetStorageTypeByName(createDto.Name);
            if (storageTypeWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_EXISTED,
                };
            }

            var newStorageType = new StorageType { Name = createDto.Name.CapitalizeAllWords() };

            await _storageTypeRepo.AddStorageType(newStorageType);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_STORAGE_TYPE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateStorageType(CreateUpdateStorageTypeDto updateDto, int targetStorageTypeId, int authRoleId)
        {
            var hasUpdateStorageTypePermission = await _storageTypeRepo.VerifyPermission(
                authRoleId,
                Permission.UPDATE_STORAGE_TYPE.ToString()
            );
            if (!hasUpdateStorageTypePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetStorageType = await _storageTypeRepo.GetStorageTypeById(targetStorageTypeId);
            if (targetStorageType == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_NOT_FOUND,
                };
            }

            var storageTypeWithSameName = await _storageTypeRepo.GetStorageTypeByName(updateDto.Name);
            if (storageTypeWithSameName != null && storageTypeWithSameName.TypeId != targetStorageTypeId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_EXISTED,
                };
            }
            targetStorageType.Name = updateDto.Name.CapitalizeAllWords();

            await _storageTypeRepo.UpdateStorageType(targetStorageType);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_STORAGE_TYPE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveStorageType(int targetStorageTypeId, int authRoleId)
        {
            var hasRemoveStorageTypePermission = await _storageTypeRepo.VerifyPermission(
                authRoleId,
                Permission.DELETE_STORAGE_TYPE.ToString()
            );
            if (!hasRemoveStorageTypePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetStorageType = await _storageTypeRepo.GetStorageTypeById(targetStorageTypeId);
            if (targetStorageType == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_NOT_FOUND,
                };
            }

            var isStorageTypeBeingUsed = await _storageTypeRepo.IsStorageTypeBeingUsed(targetStorageTypeId);
            if (isStorageTypeBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_BEING_USED,
                };
            }

            await _storageTypeRepo.DeleteStorageType(targetStorageType);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_STORAGE_TYPE_SUCCESSFULLY,
            };
        }
    }
}
