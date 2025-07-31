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
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepo;
        private readonly IRoleRepository _roleRepo;

        public OrderStatusService(IOrderStatusRepository orderStatusRepo, IRoleRepository roleRepo)
        {
            _orderStatusRepo = orderStatusRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<bool>> VerifyPermission(int authRoleId, string permission)
        {
            var isVerified = await _orderStatusRepo.VerifyPermission(authRoleId, permission);
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

        public async Task<ServiceResponse<List<OrderStatus>>> GetAllOrderStatuses(BaseQueryObject queryObject)
        {
            var (orderStatuses, total) = await _orderStatusRepo.GetAllOrderStatuses(queryObject);

            return new ServiceResponse<List<OrderStatus>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = orderStatuses,
                Total = total,
                Took = orderStatuses.Count,
            };
        }

        public async Task<ServiceResponse<OrderStatus?>> GetOrderStatusById(int orderStatusId, int authRoleId)
        {
            var hasViewOrderStatusPermission = await _roleRepo.VerifyPermission(
                authRoleId,
                Permission.ACCESS_ORDER_STATUS_DASHBOARD_PAGE.ToString()
            );
            if (!hasViewOrderStatusPermission && orderStatusId != authRoleId)
            {
                return new ServiceResponse<OrderStatus?>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var orderStatus = await _orderStatusRepo.GetOrderStatusById(orderStatusId);
            return new ServiceResponse<OrderStatus?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = orderStatus,
            };
        }

        public async Task<ServiceResponse> AddNewOrderStatus(CreateUpdateOrderStatusDto createDto, int authRoleId)
        {
            var hasAddOrderStatusPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_ORDER_STATUS.ToString());
            if (!hasAddOrderStatusPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var orderStatusWithSameName = await _orderStatusRepo.GetOrderStatusByName(createDto.Name);
            if (orderStatusWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.ROLE_EXISTED,
                };
            }

            var newOrderStatus = new OrderStatus
            {
                Name = createDto.Name.CapitalizeAllWords(),
                Description = createDto.Description ?? string.Empty,
                IsDefaultState = createDto.IsDefaultState,
                IsAccounted = createDto.IsAccounted,
                IsUnfulfilled = createDto.IsUnfulfilled,
            };

            await _orderStatusRepo.AddNewOrderStatus(newOrderStatus);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_ORDER_STATUS_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateOrderStatus(CreateUpdateOrderStatusDto updateDto, int targetOrderStatusId, int authRoleId)
        {
            var hasUpdateOrderStatusPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_ORDER_STATUS.ToString());
            if (!hasUpdateOrderStatusPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetOrderStatus = await _orderStatusRepo.GetOrderStatusById(targetOrderStatusId);
            if (targetOrderStatus == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ROLE_NOT_FOUND,
                };
            }

            var orderStatusWithSameName = await _orderStatusRepo.GetOrderStatusByName(updateDto.Name);
            if (orderStatusWithSameName != null && orderStatusWithSameName.StatusId != targetOrderStatusId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.ROLE_EXISTED,
                };
            }
            targetOrderStatus.Name = updateDto.Name.CapitalizeAllWords();
            targetOrderStatus.Description = updateDto.Description ?? string.Empty;
            targetOrderStatus.IsDefaultState = updateDto.IsDefaultState;
            targetOrderStatus.IsAccounted = updateDto.IsAccounted;
            targetOrderStatus.IsUnfulfilled = updateDto.IsUnfulfilled;

            await _orderStatusRepo.UpdateOrderStatus(targetOrderStatus);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_ORDER_STATUS_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveOrderStatus(int targetOrderStatusId, int authRoleId)
        {
            var hasRemoveOrderStatusPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DELETE_ORDER_STATUS.ToString());
            if (!hasRemoveOrderStatusPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetOrderStatus = await _orderStatusRepo.GetOrderStatusById(targetOrderStatusId);
            if (targetOrderStatus == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ROLE_NOT_FOUND,
                };
            }

            var isOrderStatusBeingUsed = await _orderStatusRepo.IsOrderStatusBeingUsed(targetOrderStatusId);
            if (isOrderStatusBeingUsed)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ROLE_BEING_USED,
                };
            }

            await _orderStatusRepo.RemoveOrderStatus(targetOrderStatus);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_ORDER_STATUS_SUCCESSFULLY,
            };
        }
    }
}
