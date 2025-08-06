using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _productService.GetAllProducts(queryObject);
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

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProductDetail([FromRoute] int productId)
        {
            var result = await _productService.GetProductDetail(productId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProductsByName([FromQuery] string searchTerm)
        {
            var result = await _productService.SearchProductsByName(searchTerm);
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

        [HttpGet("detailed-items")]
        public async Task<IActionResult> SearchProductsByName([FromQuery] List<int> ids)
        {
            var result = await _productService.GetDetailedProductItems(ids);
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
        [HttpPost]
        public async Task<IActionResult> AddNewProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _productService.AddNewProduct(createProductDto, int.Parse(authUserId!), int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{productId:int}/items")]
        public async Task<IActionResult> UpdateProductItems(
            [FromRoute] int productId,
            [FromBody] UpdateProductItemsDto updateProductItemsDto
        )
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _productService.UpdateProductItems(updateProductItemsDto, productId, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int productId)
        {
            var authRoleId = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _productService.DeleteProduct(productId, int.Parse(authRoleId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _productService.GetAllCategories(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data!.Select(c => c.ToCategoryDto()),
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }
    }
}
