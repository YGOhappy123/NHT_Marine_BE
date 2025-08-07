using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("verify-coupon")]
        public async Task<IActionResult> VerifyCoupon([FromBody] VerifyCouponDto verifyCouponDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.VerifyCoupon(verifyCouponDto, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _orderService.GetAllOrders(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data,
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetCustomerOrders([FromQuery] BaseQueryObject queryObject)
        {
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.GetCustomerOrders(queryObject, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data,
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost]
        public async Task<IActionResult> PlaceNewOrder([FromBody] PlaceOrderDto placeOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.PlaceNewOrder(placeOrderDto, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message, Data = new { result.OrderId } });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{orderId:int}/inventory")]
        public async Task<IActionResult> ChooseOrderInventory([FromRoute] int orderId, [FromBody] AcceptOrderDto acceptOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.ChooseOrderInventory(orderId, acceptOrderDto, int.Parse(authUserId!), int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.UpdateOrderStatus(
                orderId,
                updateOrderStatusDto,
                int.Parse(authUserId!),
                int.Parse(authRoleId!)
            );
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("inventory")]
        public async Task<IActionResult> GetProductItemInventories([FromQuery] List<int> ids)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _orderService.GetProductItemInventories(ids, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }
    }
}
