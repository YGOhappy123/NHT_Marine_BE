using System.Security.Claims;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(AppUser user, int? roleId = 0);
        string GenerateRefreshToken(Account account);
        string GenerateResetPasswordToken(Customer customer);
        bool VerifyRefreshToken(string refreshToken, out ClaimsPrincipal? principal);
        bool VerifyResetPasswordToken(string resetPasswordToken, out ClaimsPrincipal? principal);
    }
}
