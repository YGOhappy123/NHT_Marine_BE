using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("verify-permission")]
        public async Task<IActionResult> VerifyPermission([FromQuery] string permission)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _roleService.VerifyPermission(int.Parse(authRoleId!), permission);

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }
    }
}
