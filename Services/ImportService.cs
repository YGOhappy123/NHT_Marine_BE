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
    public class ImportService : IImportService
    {
        private readonly IImportRepository _importRepo;
        private readonly IStorageRepository _storageRepo;
        private readonly IProductRepository _productRepo;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IRoleRepository _roleRepo;

        public ImportService(
            IImportRepository importRepo,
            IStorageRepository storageRepo,
            IProductRepository productRepo,
            ISupplierRepository supplierRepo,
            IRoleRepository roleRepo
        )
        {
            _importRepo = importRepo;
            _storageRepo = storageRepo;
            _productRepo = productRepo;
            _supplierRepo = supplierRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<List<ImportDto>>> GetAllImports(BaseQueryObject queryObject)
        {
            var (imports, total) = await _importRepo.GetAllProductImports(queryObject);

            var mappedImports = imports.Select(s => s.ToImportDto()).ToList();
            foreach (var import in mappedImports)
            {
                foreach (var item in import.Items!)
                {
                    item.ProductItem = await _productRepo.GetDetailedProductItemById((int)item.ProductItemId!);
                }
            }

            return new ServiceResponse<List<ImportDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedImports,
                Total = total,
                Took = imports.Count,
            };
        }

        public async Task<ServiceResponse> TrackNewImport(TrackNewImportDto trackNewImportDto, int authUserId, int authRoleId)
        {
            var hasTrackImportPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_IMPORT.ToString());
            if (!hasTrackImportPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var supplier = await _supplierRepo.GetSupplierById(trackNewImportDto.SupplierId);
            if (supplier == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_NOT_FOUND,
                };
            }

            var newProductImport = new ProductImport
            {
                SupplierId = trackNewImportDto.SupplierId,
                InvoiceNumber = trackNewImportDto.InvoiceNumber,
                ImportDate = trackNewImportDto.ImportDate,
                TrackedBy = authRoleId,
                IsDistributed = false,
                TotalCost = 0,
                Items = [],
            };
            foreach (var item in trackNewImportDto.Items)
            {
                var productItem = await _productRepo.GetProductItemById(item.ProductItemId);
                if (productItem == null)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.PRODUCT_NOT_FOUND,
                    };
                }

                newProductImport.TotalCost += item.Quantity * item.Cost;
                newProductImport.Items.Add(
                    new ImportItem
                    {
                        ProductItemId = item.ProductItemId,
                        Cost = item.Cost,
                        Quantity = item.Quantity,
                    }
                );
            }

            await _importRepo.AddProductImport(newProductImport);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.TRACK_IMPORT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> DistributeImportItems(DistributeImportDto distributeImportDto, int importId, int authRoleId)
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

            var productImport = await _importRepo.GetProductImportById(importId);
            if (productImport == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_IMPORT_NOT_FOUND,
                };
            }
            if (productImport.IsDistributed == true)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_IMPORT_ALREADY_DISTRIBUTED,
                };
            }

            foreach (var item in distributeImportDto.Items)
            {
                var sumOfQuantity = item.Storages.Sum(s => s.Quantity);
                var matchingItem = productImport.Items.Find(ii => ii.ProductItemId == item.ProductItemId);
                if (sumOfQuantity != (matchingItem?.Quantity ?? 0))
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.PRODUCT_QUANTITY_MISMATCH,
                    };
                }

                foreach (var storage in item.Storages)
                {
                    var inventory = await _storageRepo.GetInventoryByStorageAndProductItemId(storage.StorageId, item.ProductItemId);
                    if (inventory == null)
                    {
                        await _storageRepo.AddInventory(
                            new Inventory
                            {
                                StorageId = storage.StorageId,
                                ProductItemId = item.ProductItemId,
                                Quantity = storage.Quantity,
                            }
                        );
                    }
                    else
                    {
                        inventory.Quantity += storage.Quantity;
                        await _storageRepo.UpdateInventory(inventory);
                    }
                }
            }

            productImport.IsDistributed = true;
            await _importRepo.UpdateProductImport(productImport);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_INVENTORY_SUCCESSFULLY,
            };
        }
    }
}
