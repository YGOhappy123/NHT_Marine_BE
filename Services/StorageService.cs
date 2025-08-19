using CloudinaryDotNet.Actions;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class StorageService : IStorageService
    {
        private readonly IProductRepository _productRepo;
        private readonly IStorageRepository _storageRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IStorageTypeRepository _storageTypeRepo;

        public StorageService(
            IProductRepository productRepo,
            IStorageRepository storageRepo,
            IRoleRepository roleRepo,
            IStorageTypeRepository storageTypeRepo
        )
        {
            _productRepo = productRepo;
            _storageRepo = storageRepo;
            _roleRepo = roleRepo;
            _storageTypeRepo = storageTypeRepo;
        }

        public async Task<ServiceResponse<List<StorageDto>>> GetAllStorages(BaseQueryObject queryObject)
        {
            var (storages, total) = await _storageRepo.GetAllStorages(queryObject);

            var mappedStorages = storages.Select(s => s.ToStorageDto()).ToList();
            foreach (var storage in mappedStorages)
            {
                foreach (var inventory in storage.ProductItems!)
                {
                    inventory.ProductItem = await _productRepo.GetDetailedProductItemById((int)inventory.ProductItemId!);
                }
            }

            return new ServiceResponse<List<StorageDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedStorages,
                Total = total,
                Took = storages.Count,
            };
        }

        public async Task<ServiceResponse> ChangeInventoryLocation(
            ChangeInventoryLocationDto changeInventoryDto,
            int storageId,
            int authRoleId
        )
        {
            var hasUpdateInventoryPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_INVENTORY.ToString());
            if (!hasUpdateInventoryPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var inventory = await _storageRepo.GetInventoryByStorageAndProductItemId(storageId, changeInventoryDto.ProductItemId);
            if (inventory == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.INVENTORY_NOT_FOUND,
                };
            }
            if (inventory.Quantity < changeInventoryDto.Quantity)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.INVENTORY_QUANTITY_NOT_ENOUGH,
                };
            }

            inventory.Quantity -= changeInventoryDto.Quantity;
            if (inventory.Quantity == 0)
            {
                await _storageRepo.DeleteInventory(inventory);
            }
            else
            {
                await _storageRepo.UpdateInventory(inventory);
            }

            var inventoryInTargetStorage = await _storageRepo.GetInventoryByStorageAndProductItemId(
                changeInventoryDto.NewStorageId,
                changeInventoryDto.ProductItemId
            );
            if (inventoryInTargetStorage == null)
            {
                await _storageRepo.AddInventory(
                    new Inventory
                    {
                        StorageId = changeInventoryDto.NewStorageId,
                        ProductItemId = changeInventoryDto.ProductItemId,
                        Quantity = changeInventoryDto.Quantity,
                    }
                );
            }
            else
            {
                inventoryInTargetStorage.Quantity += changeInventoryDto.Quantity;
                await _storageRepo.UpdateInventory(inventoryInTargetStorage);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_INVENTORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ChangeInventoryVariant(
            ChangeInventoryVariantDto changeInventoryDto,
            int storageId,
            int authRoleId
        )
        {
            var hasUpdateInventoryPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_INVENTORY.ToString());
            if (!hasUpdateInventoryPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var inventory = await _storageRepo.GetInventoryByStorageAndProductItemId(storageId, changeInventoryDto.ProductItemId);
            if (inventory == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.INVENTORY_NOT_FOUND,
                };
            }
            if (inventory.Quantity < changeInventoryDto.Quantity)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.INVENTORY_QUANTITY_NOT_ENOUGH,
                };
            }

            inventory.Quantity -= changeInventoryDto.Quantity;
            if (inventory.Quantity == 0)
            {
                await _storageRepo.DeleteInventory(inventory);
            }
            else
            {
                await _storageRepo.UpdateInventory(inventory);
            }

            var inventoryInTargetVariant = await _storageRepo.GetInventoryByStorageAndProductItemId(
                storageId,
                changeInventoryDto.NewProductItemId
            );
            if (inventoryInTargetVariant == null)
            {
                await _storageRepo.AddInventory(
                    new Inventory
                    {
                        StorageId = storageId,
                        ProductItemId = changeInventoryDto.NewProductItemId,
                        Quantity = changeInventoryDto.Quantity,
                    }
                );
            }
            else
            {
                inventoryInTargetVariant.Quantity += changeInventoryDto.Quantity;
                await _storageRepo.UpdateInventory(inventoryInTargetVariant);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_INVENTORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _storageRepo.VerifyPermission(authRoleId, permission);
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

        public async Task<ServiceResponse<Storage?>> GetStorageById(int storageId, int authRoleId)
        {
            var hasViewStoragePermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ACCESS_STORAGE_DASHBOARD_PAGE.ToString()
            );
            if (!hasViewStoragePermission && storageId != authRoleId)
            {
                return new ServiceResponse<Storage?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var storage = await _storageRepo.GetStorageById(storageId);
            return new ServiceResponse<Storage?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = storage,
            };
        }

        public async Task<ServiceResponse> AddNewStorage(CreateUpdateStorageDto createDto, int authRoleId)
        {
            var hasAddStoragePermission = await _storageRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_STORAGE.ToString());
            if (!hasAddStoragePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var storageWithSameName = await _storageRepo.GetStorageByName(createDto.Name);
            if (storageWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.STORAGE_EXISTED,
                };
            }

            var type = await _storageTypeRepo.GetStorageTypeById(createDto.TypeId);
            if (type == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_NOT_EXIST,
                };
            }

            var newStorage = new Storage { Name = createDto.Name.CapitalizeAllWords(), TypeId = createDto.TypeId };

            await _storageRepo.AddStorage(newStorage);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_STORAGE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateStorage(CreateUpdateStorageDto updateDto, int targetStorageId, int authRoleId)
        {
            var hasUpdateStoragePermission = await _storageRepo.VerifyPermission(authRoleId, Permission.UPDATE_STORAGE.ToString());
            if (!hasUpdateStoragePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetStorage = await _storageRepo.GetStorageById(targetStorageId);
            if (targetStorage == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.STORAGE_NOT_FOUND,
                };
            }

            var storageWithSameName = await _storageRepo.GetStorageByName(updateDto.Name);
            if (storageWithSameName != null && storageWithSameName.StorageId != targetStorageId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.STORAGE_EXISTED,
                };
            }

            var type = await _storageTypeRepo.GetStorageTypeById(updateDto.TypeId);
            if (type == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.STORAGE_TYPE_NOT_EXIST,
                };
            }

            targetStorage.Name = updateDto.Name.CapitalizeAllWords();
            targetStorage.TypeId = updateDto.TypeId;

            await _storageRepo.UpdateStorage(targetStorage);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_STORAGE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveStorage(int targetStorageId, int authRoleId)
        {
            var hasRemoveStoragePermission = await _storageRepo.VerifyPermission(authRoleId, Permission.DELETE_STORAGE.ToString());
            if (!hasRemoveStoragePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetStorage = await _storageRepo.GetStorageById(targetStorageId);
            if (targetStorage == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.STORAGE_NOT_FOUND,
                };
            }

            var isStorageBeingUsed = await _storageRepo.IsStorageBeingUsed(targetStorageId);
            if (isStorageBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.STORAGE_BEING_USED,
                };
            }

            await _storageRepo.DeleteStorage(targetStorage);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_STORAGE_SUCCESSFULLY,
            };
        }
    }
}
