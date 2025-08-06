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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepo;
        private readonly IRoleRepository _roleRepo;

        public SupplierService(ISupplierRepository supplierRepo, IRoleRepository roleRepo)
        {
            _supplierRepo = supplierRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _supplierRepo.VerifyPermission(authRoleId, permission);
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

        public async Task<ServiceResponse<List<Supplier>>> GetAllSuppliers(BaseQueryObject queryObject)
        {
            var (suppliers, total) = await _supplierRepo.GetAllSuppliers(queryObject);

            return new ServiceResponse<List<Supplier>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = suppliers,
                Total = total,
                Took = suppliers.Count,
            };
        }

        public async Task<ServiceResponse<Supplier?>> GetSupplierById(int supplierId, int authRoleId)
        {
            var hasViewSSupplierPermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ACCESS_SUPPLIER_DASHBOARD_PAGE.ToString()
            );
            if (!hasViewSSupplierPermission && supplierId != authRoleId)
            {
                return new ServiceResponse<Supplier?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var supplier = await _supplierRepo.GetSupplierById(supplierId);
            return new ServiceResponse<Supplier?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = supplier,
            };
        }

        public async Task<ServiceResponse> AddNewSupplier(CreateUpdateSupplierDto createDto, int authRoleId)
        {
            var hasAddSSupplierPermission = await _supplierRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_SUPPLIER.ToString());
            if (!hasAddSSupplierPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var supplierWithSameName = await _supplierRepo.GetSupplierByName(createDto.Name);
            if (supplierWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_EXISTED,
                };
            }

            var supplierWithSameAddress = await _supplierRepo.GetSupplierByAddress(createDto.Address);
            if (supplierWithSameAddress != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_ADDRESS_EXISTED,
                };
            }

            var supplierWithSameEmail = await _supplierRepo.GetSupplierByContactEmail(createDto.ContactEmail);
            if (supplierWithSameEmail != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_CONTACT_EMAIL_EXISTED,
                };
            }

            var supplierWithSamePhone = await _supplierRepo.GetSupplierByContactPhone(createDto.ContactPhone);
            if (supplierWithSamePhone != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_CONTACT_PHONE_EXISTED,
                };
            }

            var newSupplier = new Supplier
            {
                Name = createDto.Name.CapitalizeAllWords(),
                Address = createDto.Address,
                ContactEmail = createDto.ContactEmail,
                ContactPhone = createDto.ContactPhone,
            };

            await _supplierRepo.AddSupplier(newSupplier);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_SUPPLIER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateSupplier(CreateUpdateSupplierDto updateDto, int targetSupplierId, int authRoleId)
        {
            var hasUpdateSSupplierPermission = await _supplierRepo.VerifyPermission(authRoleId, Permission.UPDATE_SUPPLIER.ToString());
            if (!hasUpdateSSupplierPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetSupplier = await _supplierRepo.GetSupplierById(targetSupplierId);
            if (targetSupplier == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_NOT_FOUND,
                };
            }

            var supplierWithSameName = await _supplierRepo.GetSupplierByName(updateDto.Name);
            if (supplierWithSameName != null && supplierWithSameName.SupplierId != targetSupplierId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_EXISTED,
                };
            }

            var supplierWithSameAddress = await _supplierRepo.GetSupplierByAddress(updateDto.Address);
            if (supplierWithSameAddress != null && supplierWithSameAddress.SupplierId != targetSupplierId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_ADDRESS_EXISTED,
                };
            }

            var supplierWithSameEmail = await _supplierRepo.GetSupplierByContactEmail(updateDto.ContactEmail);
            if (supplierWithSameEmail != null && supplierWithSameEmail.SupplierId != targetSupplierId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_CONTACT_EMAIL_EXISTED,
                };
            }

            var supplierWithSamePhone = await _supplierRepo.GetSupplierByContactPhone(updateDto.ContactPhone);
            if (supplierWithSamePhone != null && supplierWithSamePhone.SupplierId != targetSupplierId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_CONTACT_PHONE_EXISTED,
                };
            }

            targetSupplier.Name = updateDto.Name.CapitalizeAllWords();
            targetSupplier.Address = updateDto.Address;
            targetSupplier.ContactEmail = updateDto.ContactEmail;
            targetSupplier.ContactPhone = updateDto.ContactPhone;

            await _supplierRepo.UpdateSupplier(targetSupplier);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_SUPPLIER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveSupplier(int targetSupplierId, int authRoleId)
        {
            var hasRemoveSSupplierPermission = await _supplierRepo.VerifyPermission(authRoleId, Permission.DELETE_SUPPLIER.ToString());
            if (!hasRemoveSSupplierPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetSupplier = await _supplierRepo.GetSupplierById(targetSupplierId);
            if (targetSupplier == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_NOT_FOUND,
                };
            }

            var isSupplierBeingUsed = await _supplierRepo.IsSupplierBeingUsed(targetSupplierId);
            if (isSupplierBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.SUPPLIER_BEING_USED,
                };
            }

            await _supplierRepo.DeleteSupplier(targetSupplier);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_SUPPLIER_SUCCESSFULLY,
            };
        }
    }
}
