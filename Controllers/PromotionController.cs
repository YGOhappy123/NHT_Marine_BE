using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/promotions")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly IRoleService _roleService;

        public PromotionController(IPromotionService promotionService, IRoleService roleService)
        {
            _promotionService = promotionService;
            _roleService = roleService;
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllPromotions([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _promotionService.GetAllPromotions(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data!.Select(rp => rp),
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public async Task<IActionResult> AddNewPromotion([FromBody] CreateUpdatePromotionDto createPromotionDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _promotionService.AddNewPromotion(createPromotionDto, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("coupons")]
        public async Task<IActionResult> GetAllCoupons([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _promotionService.GetAllCoupons(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data!.Select(rp => rp.ToCouponDto()),
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }
    }
}
