using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.File;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadSingleImage([FromForm] UploadImageDto uploadImageDto, [FromQuery] string? folder)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _fileService.UploadImageToCloudinary(uploadImageDto.File, folder);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message, Data = new { result.ImageUrl } });
        }

        [Authorize]
        [HttpPost("upload-base64-image")]
        public async Task<IActionResult> UploadBase64Image(UploadBase64ImageDto uploadImageDto, [FromQuery] string? folder)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _fileService.UploadBase64ImageToCloudinary(uploadImageDto.Base64Image, folder);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message, Data = new { result.ImageUrl } });
        }

        [Authorize]
        [HttpPost("delete-image")]
        public async Task<IActionResult> DeleteSingleImage([FromBody] DeleteImageDto deleteImageDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _fileService.DeleteImageFromCloudinary(deleteImageDto.ImageUrl);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }
    }
}
