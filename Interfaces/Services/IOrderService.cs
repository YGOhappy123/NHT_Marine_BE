using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse<Coupon?>> VerifyCoupon(VerifyCouponDto verifyCouponDto, int customerId);
        Task<ServiceResponse<List<OrderDto>>> GetAllOrders(BaseQueryObject queryObject);
        Task<ServiceResponse<List<OrderDto>>> GetCustomerOrders(BaseQueryObject queryObject, int customerId);
        Task<ServiceResponse> PlaceNewOrder(PlaceOrderDto placeOrderDto, int customerId);
        Task<ServiceResponse> ChooseOrderInventory(int orderId, AcceptOrderDto acceptOrderDto, int authUserId, int authRoleId);
        Task<ServiceResponse> UpdateOrderStatus(int orderId, UpdateOrderStatusDto updateOrderStatusDto, int authUserId, int authRoleId);
        Task<ServiceResponse<List<ProductInventoryDto>>> GetProductItemInventories(List<int> productItemIds, int authRoleId);
    }
}
