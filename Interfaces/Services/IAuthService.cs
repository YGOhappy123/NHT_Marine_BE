using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<Customer>> CustomerSignIn(SignInDto signInDto);
        Task<ServiceResponse<Staff>> StaffSignIn(SignInDto signInDto);
        // Task<ServiceResponse<Guest>> SignUpGuestAccount(SignUpDto signUpDto);
        // Task<ServiceResponse> RefreshToken(RefreshTokenDto refreshTokenDto);
        // Task<ServiceResponse> ChangePassword(ChangePasswordDto changePasswordDto, int authUserId, string authUserRole);
        // Task<ServiceResponse> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        // Task<ServiceResponse> ResetPassword(ResetPasswordDto resetPasswordDto);
        // Task<ServiceResponse<Guest>> GoogleAuthentication(GoogleAuthDto googleAuthDto);
        // Task<ServiceResponse> DeactivateAccount(DeactivateAccountDto deactivateAccountDto, int authUserId, string authUserRole);
    }
}
