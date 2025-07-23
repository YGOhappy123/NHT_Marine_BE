using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Services;

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
                    Data = result.Data!.Select(rp => rp.ToRootProductDto()),
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

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data?.ToRootProductDto() });
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
