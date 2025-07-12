using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtService(IConfiguration configuration, JwtSecurityTokenHandler tokenHandler)
        {
            _configuration = configuration;
            _tokenHandler = tokenHandler;
        }

        private string GenerateToken(Claim[] claims, string tokenKeyPath, int expirationInMins = 60)
        {
            var key = Encoding.ASCII.GetBytes(_configuration[tokenKeyPath]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(expirationInMins),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        public string GenerateAccessToken(AppUser user, int? roleId = 0)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            var claims =
                user is Customer
                    ? new[] { new Claim(ClaimTypes.Name, ((Customer)user).CustomerId.ToString()) }
                    : new[]
                    {
                        new Claim(ClaimTypes.Name, ((Staff)user).StaffId.ToString()),
                        new Claim(ClaimTypes.Role, roleId.ToString()!),
                    };

            return GenerateToken(claims, "Jwt:AccessTokenSecret", 30);
        }

        public string GenerateRefreshToken(Account account)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, account.AccountId.ToString()) };

            return GenerateToken(claims, "Jwt:RefreshTokenSecret", 60 * 24 * 7);
        }

        public string GenerateResetPasswordToken(Customer customer)
        {
            var claims = new[] { new Claim(ClaimTypes.Email, customer.Email!.ToString()) };

            return GenerateToken(claims, "Jwt:ResetPasswordTokenSecret", 10);
        }

        private ClaimsPrincipal? VerifyToken(string token, string tokenKeyPath)
        {
            var key = Encoding.ASCII.GetBytes(_configuration[tokenKeyPath]!);

            try
            {
                var principal = _tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken
                );

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool VerifyRefreshToken(string refreshToken, out ClaimsPrincipal? principal)
        {
            principal = VerifyToken(refreshToken, "Jwt:RefreshTokenSecret");
            return principal != null;
        }

        public bool VerifyResetPasswordToken(string resetPasswordToken, out ClaimsPrincipal? principal)
        {
            principal = VerifyToken(resetPasswordToken, "Jwt:ResetPasswordTokenSecret");
            return principal != null;
        }
    }
}
