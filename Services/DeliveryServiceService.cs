using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class DeliveryServiceService : IDeliveryServiceService
    {
        private readonly IDeliveryServiceRepository _deliveryServiceRepo;
        private readonly IRoleRepository _roleRepo;

        public DeliveryServiceService(IDeliveryServiceRepository deliveryServiceRepo, IRoleRepository roleRepo)
        {
            _deliveryServiceRepo = deliveryServiceRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _deliveryServiceRepo.VerifyPermission(authRoleId, permission);
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

        public async Task<ServiceResponse<List<DeliveryService>>> GetAllDeliveryServices(BaseQueryObject queryObject)
        {
            var (deliveryServices, total) = await _deliveryServiceRepo.GetAllDeliveryServices(queryObject);

            return new ServiceResponse<List<DeliveryService>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = deliveryServices,
                Total = total,
                Took = deliveryServices.Count,
            };
        }

        public async Task<ServiceResponse<DeliveryService?>> GetDeliveryServiceById(int deliveryServiceId, int authRoleId)
        {
            var hasViewDeliveryServicePermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ACCESS_DELIVERY_SERVICE_DASHBOARD_PAGE.ToString()
            );
            if (!hasViewDeliveryServicePermission && deliveryServiceId != authRoleId)
            {
                return new ServiceResponse<DeliveryService?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var deliveryService = await _deliveryServiceRepo.GetDeliveryServiceById(deliveryServiceId);
            return new ServiceResponse<DeliveryService?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = deliveryService,
            };
        }

        public async Task<ServiceResponse> AddNewDeliveryService(CreateUpdateDeliveryServiceDto createDto, int authRoleId)
        {
            var hasAddDeliveryServicePermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ADD_NEW_DELIVERY_SERVICE.ToString()
            );
            if (!hasAddDeliveryServicePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var deliveryServiceWithSameName = await _deliveryServiceRepo.GetDeliveryServiceByName(createDto.Name);
            if (deliveryServiceWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.ROLE_EXISTED,
                };
            }

            var newDeliveryService = new DeliveryService
            {
                Name = createDto.Name.CapitalizeAllWords(),
                ContactPhone = createDto.ContactPhone,
            };

            await _deliveryServiceRepo.AddNewDeliveryService(newDeliveryService);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_ROLE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateDeliveryService(
            CreateUpdateDeliveryServiceDto updateDto,
            int targetDeliveryServiceId,
            int authRoleId
        )
        {
            var hasUpdateDeliveryServicePermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.UPDATE_DELIVERY_SERVICE.ToString()
            );
            if (!hasUpdateDeliveryServicePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetDeliveryService = await _deliveryServiceRepo.GetDeliveryServiceById(targetDeliveryServiceId);
            if (targetDeliveryService == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ROLE_NOT_FOUND,
                };
            }

            var deliveryServiceWithSameName = await _deliveryServiceRepo.GetDeliveryServiceByName(updateDto.Name);
            if (deliveryServiceWithSameName != null && deliveryServiceWithSameName.ServiceId != targetDeliveryServiceId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.ROLE_EXISTED,
                };
            }
            targetDeliveryService.Name = updateDto.Name.CapitalizeAllWords();
            targetDeliveryService.ContactPhone = updateDto.ContactPhone;

            await _deliveryServiceRepo.UpdateDeliveryService(targetDeliveryService);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_ROLE_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveDeliveryService(int targetDeliveryServiceId, int authRoleId)
        {
            var hasRemoveDeliveryServicePermission = await _deliveryServiceRepo.VerifyPermission(
                authRoleId,
                Permission.DELETE_DELIVERY_SERVICE.ToString()
            );
            if (!hasRemoveDeliveryServicePermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetDeliveryService = await _deliveryServiceRepo.GetDeliveryServiceById(targetDeliveryServiceId);
            if (targetDeliveryService == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ROLE_NOT_FOUND,
                };
            }

            var isDeliveryServiceBeingUsed = await _deliveryServiceRepo.IsDeliveryServiceBeingUsed(targetDeliveryServiceId);
            if (isDeliveryServiceBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ROLE_BEING_USED,
                };
            }

            await _deliveryServiceRepo.RemoveDeliveryService(targetDeliveryService);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_ROLE_SUCCESSFULLY,
            };
        }
    }
}
