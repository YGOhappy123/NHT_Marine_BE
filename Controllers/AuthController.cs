using Microsoft.AspNetCore.Mvc;
using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> CustomerSignIn([FromBody] SignInDto signInDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _authService.CustomerSignIn(signInDto);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Message = result.Message,
                    Data = new
                    {
                        User = result.Data!.ToCustomerDto(),
                        result.AccessToken,
                        result.RefreshToken,
                    },
                }
            );
        }

        [HttpPost("dashboard/sign-in")]
        public async Task<IActionResult> StaffSignIn([FromBody] SignInDto signInDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _authService.StaffSignIn(signInDto);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Message = result.Message,
                    Data = new
                    {
                        User = result.Data!.ToStaffDto(),
                        result.AccessToken,
                        result.RefreshToken,
                    },
                }
            );
        }

        // [HttpPost("sign-up")]
        // public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var result = await _authService.SignUpGuestAccount(signUpDto);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(
        //         result.Status,
        //         new SuccessResponseDto
        //         {
        //             Message = result.Message,
        //             Data = new
        //             {
        //                 User = result.Data!.ToGuestDto(),
        //                 result.AccessToken,
        //                 result.RefreshToken,
        //             },
        //         }
        //     );
        // }

        // [HttpPost("refresh-token")]
        // public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var result = await _authService.RefreshToken(refreshTokenDto);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message, Data = new { result.AccessToken } });
        // }

        // [Authorize]
        // [HttpPost("change-password")]
        // public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        //     var authUserRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        //     var result = await _authService.ChangePassword(changePasswordDto, int.Parse(authUserId!), authUserRole!);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        // }

        // [HttpPost("forgot-password")]
        // public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var result = await _authService.ForgotPassword(forgotPasswordDto);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        // }

        // [HttpPost("reset-password")]
        // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var result = await _authService.ResetPassword(resetPasswordDto);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        // }

        // [HttpPost("google-auth")]
        // public async Task<IActionResult> GoogleAuthentication([FromBody] GoogleAuthDto googleAuthDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var result = await _authService.GoogleAuthentication(googleAuthDto);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(
        //         result.Status,
        //         new SuccessResponseDto
        //         {
        //             Message = result.Message,
        //             Data = new
        //             {
        //                 User = result.Data!.ToGuestDto(),
        //                 result.AccessToken,
        //                 result.RefreshToken,
        //             },
        //         }
        //     );
        // }

        // [Authorize]
        // [HttpPost("deactivate-account")]
        // public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountDto deactivateAccountDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return StatusCode(
        //             ResStatusCode.UNPROCESSABLE_ENTITY,
        //             new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
        //         );
        //     }

        //     var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        //     var authUserRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        //     var result = await _authService.DeactivateAccount(deactivateAccountDto, int.Parse(authUserId!), authUserRole!);
        //     if (!result.Success)
        //     {
        //         return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
        //     }

        //     return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        // }
    }
}
