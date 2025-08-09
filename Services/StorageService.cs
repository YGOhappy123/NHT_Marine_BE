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

        public StorageService(IProductRepository productRepo, IStorageRepository storageRepo, IRoleRepository roleRepo)
        {
            _productRepo = productRepo;
            _storageRepo = storageRepo;
            _roleRepo = roleRepo;
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
    }
}
