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
    [Route("/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
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

            var result = await _supplierService.VerifyPermission(int.Parse(authRoleId!), permission);

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _supplierService.GetAllSuppliers(queryObject);
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

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("{supplierId:int}")]
        public async Task<IActionResult> GetSupplierById([FromRoute] int supplierId)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _supplierService.GetSupplierById(supplierId, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public async Task<IActionResult> AddNewSupplier([FromBody] CreateUpdateSupplierDto createSupplierDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _supplierService.AddNewSupplier(createSupplierDto, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{supplierId:int}")]
        public async Task<IActionResult> UpdateSupplier([FromRoute] int supplierId, [FromBody] CreateUpdateSupplierDto updateSupplierDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _supplierService.UpdateSupplier(updateSupplierDto, supplierId, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{supplierId:int}")]
        public async Task<IActionResult> RemoveSupplier([FromRoute] int supplierId)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _supplierService.RemoveSupplier(supplierId, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }
    }
}
