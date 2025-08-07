using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public enum CouponValidationResult
    {
        Valid,
        NotFound,
        InactiveOrExpired,
        MaxUsageReached,
        AlreadyUsed,
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderStatusRepository _orderStatusRepo;
        private readonly ICouponRepository _couponRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IStorageRepository _storageRepo;

        public OrderService(
            IOrderRepository orderRepo,
            IOrderStatusRepository orderStatusRepo,
            ICouponRepository couponRepo,
            IProductRepository productRepo,
            ICartRepository cartRepo,
            IRoleRepository roleRepo,
            IStorageRepository storageRepo
        )
        {
            _orderRepo = orderRepo;
            _orderStatusRepo = orderStatusRepo;
            _couponRepo = couponRepo;
            _productRepo = productRepo;
            _cartRepo = cartRepo;
            _roleRepo = roleRepo;
            _storageRepo = storageRepo;
        }

        private async Task<int> CalculateDiscountRateAsync(int productId)
        {
            var availablePromotions = await _productRepo.GetProductAvailablePromotions(productId);
            if (availablePromotions != null && availablePromotions.Count > 0)
            {
                return availablePromotions[0].DiscountRate;
            }

            return 0;
        }

        private async Task<(CouponValidationResult Result, Coupon? Coupon)> ValidateCouponCore(string code, int customerId)
        {
            var coupon = await _couponRepo.GetCouponByCode(code);
            if (coupon == null)
                return (CouponValidationResult.NotFound, null);

            var now = TimestampHandler.GetNow();
            if (!coupon.IsActive || now > coupon.ExpiredAt)
                return (CouponValidationResult.InactiveOrExpired, null);

            if (coupon.MaxUsage is not null)
            {
                var usage = await _couponRepo.CountCouponUsage(coupon.CouponId);
                if (usage >= coupon.MaxUsage)
                    return (CouponValidationResult.MaxUsageReached, null);
            }

            var used = await _couponRepo.CheckCustomerCouponUsed(coupon.CouponId, customerId);
            if (used)
                return (CouponValidationResult.AlreadyUsed, null);

            return (CouponValidationResult.Valid, coupon);
        }

        public async Task<ServiceResponse<Coupon?>> VerifyCoupon(VerifyCouponDto verifyCouponDto, int customerId)
        {
            var (result, coupon) = await ValidateCouponCore(verifyCouponDto.Code, customerId);

            switch (result)
            {
                case CouponValidationResult.Valid:
                    return new ServiceResponse<Coupon?>
                    {
                        Success = true,
                        Status = ResStatusCode.OK,
                        Data = coupon,
                    };

                case CouponValidationResult.NotFound:
                    return new ServiceResponse<Coupon?>
                    {
                        Success = false,
                        Status = ResStatusCode.NOT_FOUND,
                        Message = ErrorMessage.COUPON_NOT_FOUND,
                    };

                case CouponValidationResult.InactiveOrExpired:
                    return new ServiceResponse<Coupon?>
                    {
                        Success = false,
                        Status = ResStatusCode.BAD_REQUEST,
                        Message = ErrorMessage.COUPON_NO_LONGER_AVAILABLE,
                    };

                case CouponValidationResult.MaxUsageReached:
                    return new ServiceResponse<Coupon?>
                    {
                        Success = false,
                        Status = ResStatusCode.BAD_REQUEST,
                        Message = ErrorMessage.COUPON_REACH_MAX_USAGE,
                    };

                case CouponValidationResult.AlreadyUsed:
                    return new ServiceResponse<Coupon?>
                    {
                        Success = false,
                        Status = ResStatusCode.BAD_REQUEST,
                        Message = ErrorMessage.YOU_HAVE_USED_COUPON,
                    };

                default:
                    throw new InvalidOperationException("Unknown coupon validation result.");
            }
        }

        public async Task<ServiceResponse<List<OrderDto>>> GetAllOrders(BaseQueryObject queryObject)
        {
            var (orders, total) = await _orderRepo.GetAllOrders(queryObject);

            var mappedOrders = orders.Select(o => o.ToOrderDto()).ToList();
            foreach (var order in mappedOrders)
            {
                var availableTransitions = await _orderRepo.GetStatusTransitions((int)order.OrderStatusId!);
                order.Transitions =
                [
                    .. availableTransitions.Select(trans => new StatusTransitionDataDto
                    {
                        TransitionId = trans.TransitionId,
                        ToStatusId = trans.ToStatusId,
                        TransitionLabel = trans.TransitionLabel,
                        ToStatus = trans.ToStatus,
                    }),
                ];

                var statusUpdateLogs = await _orderRepo.GetStatusUpdateLogs(order.OrderId);
                order.UpdateLogs =
                [
                    .. statusUpdateLogs.Select(log => new StatusUpdateLogDataDto
                    {
                        LogId = log.LogId,
                        OrderId = log.OrderId,
                        StatusId = log.StatusId,
                        UpdatedAt = log.UpdatedAt,
                        UpdatedBy = log.UpdatedBy,
                        UpdatedByStaff = log.UpdatedByStaff?.ToStaffDto(),
                        Status = log.Status,
                    }),
                ];

                foreach (var orderItem in order.Items)
                {
                    orderItem.ProductItem = await _productRepo.GetDetailedProductItemById((int)orderItem.ProductItemId!);
                }
            }

            return new ServiceResponse<List<OrderDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedOrders,
                Total = total,
                Took = orders.Count,
            };
        }

        public async Task<ServiceResponse<List<OrderDto>>> GetCustomerOrders(BaseQueryObject queryObject, int customerId)
        {
            var (orders, total) = await _orderRepo.GetCustomerOrders(queryObject, customerId);

            var mappedOrders = orders.Select(o => o.ToOrderDto()).ToList();
            foreach (var order in mappedOrders)
            {
                var statusUpdateLogs = await _orderRepo.GetStatusUpdateLogs(order.OrderId);
                order.UpdateLogs =
                [
                    .. statusUpdateLogs.Select(log => new StatusUpdateLogDataDto
                    {
                        LogId = log.LogId,
                        OrderId = log.OrderId,
                        StatusId = log.StatusId,
                        UpdatedAt = log.UpdatedAt,
                        UpdatedBy = log.UpdatedBy,
                        UpdatedByStaff = log.UpdatedByStaff?.ToStaffDto(),
                        Status = log.Status,
                    }),
                ];

                foreach (var orderItem in order.Items)
                {
                    orderItem.ProductItem = await _productRepo.GetDetailedProductItemById((int)orderItem.ProductItemId!);
                }
            }

            return new ServiceResponse<List<OrderDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedOrders,
                Total = total,
                Took = orders.Count,
            };
        }

        public async Task<ServiceResponse> PlaceNewOrder(PlaceOrderDto placeOrderDto, int customerId)
        {
            var defaultOrderStatus = await _orderRepo.GetDefaultOrderStatus();
            if (defaultOrderStatus == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.INTERNAL_SERVER_ERROR,
                    Success = false,
                    Message = ErrorMessage.ORDER_SYSTEM_TEMPORARILY_UNAVAILABLE,
                };
            }

            var newOrder = new Order
            {
                CustomerId = customerId,
                OrderStatusId = defaultOrderStatus.StatusId,
                TotalAmount = 0,
                Items = [],
            };
            foreach (var item in placeOrderDto.Items)
            {
                var currentStock = await _productRepo.GetProductItemCurrentStock(item.ProductItemId);
                if (item.Quantity > currentStock)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.QUANTITY_EXCEED_CURRENT_STOCK,
                    };
                }

                var productItem = await _productRepo.GetProductItemById(item.ProductItemId);
                if (productItem == null)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.NOT_FOUND,
                        Success = false,
                        Message = ErrorMessage.PRODUCT_NOT_FOUND,
                    };
                }

                var discountRate = await CalculateDiscountRateAsync((int)productItem.RootProductId!);
                var productUnitPrice = productItem.Price * (1 - (decimal)discountRate / 100);

                newOrder.TotalAmount += item.Quantity * productUnitPrice;
                newOrder.Items.Add(
                    new OrderItem
                    {
                        ProductItemId = item.ProductItemId,
                        Quantity = item.Quantity,
                        Price = productUnitPrice,
                    }
                );
            }

            if (!string.IsNullOrEmpty(placeOrderDto.RecipientName))
            {
                newOrder.RecipientName = placeOrderDto.RecipientName.Trim();
            }

            if (!string.IsNullOrEmpty(placeOrderDto.DeliveryPhone))
            {
                newOrder.DeliveryPhone = placeOrderDto.DeliveryPhone.Trim();
            }

            if (!string.IsNullOrEmpty(placeOrderDto.DeliveryAddress))
            {
                newOrder.DeliveryAddress = placeOrderDto.DeliveryAddress.Trim();
            }

            if (!string.IsNullOrEmpty(placeOrderDto.Note))
            {
                newOrder.Note = placeOrderDto.Note.Trim();
            }

            if (!string.IsNullOrEmpty(placeOrderDto.Coupon))
            {
                var (result, coupon) = await ValidateCouponCore(placeOrderDto.Coupon, customerId);
                if (result == CouponValidationResult.Valid)
                {
                    newOrder.CouponId = coupon!.CouponId;
                    if (coupon!.Type == CouponType.Percentage)
                    {
                        newOrder.TotalAmount -= newOrder.TotalAmount * (coupon!.Amount / 100);
                    }
                    else
                    {
                        newOrder.TotalAmount -= Math.Min(newOrder.TotalAmount, coupon!.Amount);
                    }
                }
            }

            await _orderRepo.AddOrder(newOrder);
            await _cartRepo.ConvertActiveCart(customerId);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.PLACE_ORDER_SUCCESSFULLY,
                OrderId = newOrder.OrderId,
            };
        }

        public async Task<ServiceResponse> ChooseOrderInventory(int orderId, AcceptOrderDto acceptOrderDto, int authUserId, int authRoleId)
        {
            var hasProcessOrderPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.PROCESS_ORDER.ToString());
            if (!hasProcessOrderPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetOrder = await _orderRepo.GetOrderById(orderId);
            if (targetOrder == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ORDER_NOT_FOUND,
                };
            }

            var orderStatus = await _orderStatusRepo.GetOrderStatusById(acceptOrderDto.StatusId);
            if (orderStatus == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ORDER_STATUS_NOT_FOUND,
                };
            }

            var isValidTransition = await _orderStatusRepo.CheckValidStatusTransition(
                (int)targetOrder.OrderStatusId!,
                acceptOrderDto.StatusId
            );
            if (!isValidTransition || !targetOrder.OrderStatus!.IsDefaultState || orderStatus.IsUnfulfilled)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.CANNOT_UPDATE_TO_THIS_STATUS,
                };
            }

            targetOrder.OrderStatusId = acceptOrderDto.StatusId;
            await _orderRepo.ProcessOrderInventory(acceptOrderDto);
            await _orderRepo.UpdateOrder(targetOrder);
            await _orderRepo.AddStatusUpdateLog(
                new OrderStatusUpdateLog
                {
                    OrderId = orderId,
                    StatusId = acceptOrderDto.StatusId,
                    UpdatedBy = authUserId,
                }
            );

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_ORDER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateOrderStatus(
            int orderId,
            UpdateOrderStatusDto updateOrderStatusDto,
            int authUserId,
            int authRoleId
        )
        {
            var hasProcessOrderPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.PROCESS_ORDER.ToString());
            if (!hasProcessOrderPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetOrder = await _orderRepo.GetOrderById(orderId);
            if (targetOrder == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ORDER_NOT_FOUND,
                };
            }

            var orderStatus = await _orderStatusRepo.GetOrderStatusById(updateOrderStatusDto.StatusId);
            if (orderStatus == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.ORDER_STATUS_NOT_FOUND,
                };
            }

            var isValidTransition = await _orderStatusRepo.CheckValidStatusTransition(
                (int)targetOrder.OrderStatusId!,
                updateOrderStatusDto.StatusId
            );
            if (!isValidTransition || (targetOrder.OrderStatus!.IsDefaultState && !orderStatus.IsUnfulfilled))
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ErrorMessage.CANNOT_UPDATE_TO_THIS_STATUS,
                };
            }

            targetOrder.OrderStatusId = updateOrderStatusDto.StatusId;
            await _orderRepo.UpdateOrder(targetOrder);
            await _orderRepo.AddStatusUpdateLog(
                new OrderStatusUpdateLog
                {
                    OrderId = orderId,
                    StatusId = updateOrderStatusDto.StatusId,
                    UpdatedBy = authUserId,
                }
            );

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_ORDER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<ProductInventoryDto>>> GetProductItemInventories(List<int> productItemIds, int authRoleId)
        {
            var hasProcessOrderPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.PROCESS_ORDER.ToString());
            if (!hasProcessOrderPermission)
            {
                return new ServiceResponse<List<ProductInventoryDto>>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var inventories = await _storageRepo.GetProductItemInventories(productItemIds);
            return new ServiceResponse<List<ProductInventoryDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = inventories,
            };
        }
    }
}
