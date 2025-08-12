using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Interfaces.Services;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/statistics")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryStatistic([FromQuery] string type)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _statisticService.GetSummaryStatistic(type, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("popular-customers")]
        public async Task<IActionResult> GetPopularCustomerStatistic([FromQuery] string type)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _statisticService.GetPopularStatistic(type, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("revenues")]
        public async Task<IActionResult> GetRevenuesChart([FromQuery] string type)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _statisticService.GetRevenuesChart(type, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("products/{productId:int}")]
        public async Task<IActionResult> GetProductStatistic([FromRoute] int productId)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _statisticService.GetProductStatistic(productId, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }
    }
}
