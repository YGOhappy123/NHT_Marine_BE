using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.User;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IJwtService _jwtService;
        private readonly IMailerService _mailerService;

        public AuthService(
            IConfiguration configuration,
            IAccountRepository accountRepo,
            ICustomerRepository customerRepo,
            IStaffRepository staffRepo,
            IJwtService jwtService,
            IMailerService mailerService
        )
        {
            _configuration = configuration;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _staffRepo = staffRepo;
            _jwtService = jwtService;
            _mailerService = mailerService;
        }

        public async Task<ServiceResponse<Customer>> CustomerSignIn(SignInDto signInDto)
        {
            var existedAccount = await _accountRepo.GetAccountByUsername(signInDto.Username);
            if (existedAccount == null || !BCrypt.Net.BCrypt.Verify(signInDto.Password, existedAccount.Password))
            {
                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INCORRECT_USERNAME_OR_PASSWORD,
                };
            }

            var customer = await _customerRepo.GetCustomerByAccountId(existedAccount.AccountId);
            if (customer == null)
            {
                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INCORRECT_USERNAME_OR_PASSWORD,
                };
            }

            return new ServiceResponse<Customer>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.SIGN_IN_SUCCESSFULLY,
                Data = customer,
                AccessToken = _jwtService.GenerateAccessToken(customer),
                RefreshToken = _jwtService.GenerateRefreshToken(existedAccount),
            };
        }

        public async Task<ServiceResponse<Staff>> StaffSignIn(SignInDto signInDto)
        {
            var existedAccount = await _accountRepo.GetAccountByUsername(signInDto.Username);
            if (existedAccount == null || !BCrypt.Net.BCrypt.Verify(signInDto.Password, existedAccount.Password))
            {
                return new ServiceResponse<Staff>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INCORRECT_USERNAME_OR_PASSWORD,
                };
            }

            var staff = await _staffRepo.GetStaffByAccountId(existedAccount.AccountId);
            if (staff == null)
            {
                return new ServiceResponse<Staff>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INCORRECT_USERNAME_OR_PASSWORD,
                };
            }

            return new ServiceResponse<Staff>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.SIGN_IN_SUCCESSFULLY,
                Data = staff,
                AccessToken = _jwtService.GenerateAccessToken(staff, staff.RoleId),
                RefreshToken = _jwtService.GenerateRefreshToken(existedAccount),
            };
        }

        public async Task<ServiceResponse<Customer>> CustomerSignUp(SignUpDto signUpDto)
        {
            Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("customer0006"));
            var existedAccount = await _accountRepo.GetAccountByUsername(signUpDto.Username);
            if (existedAccount != null)
            {
                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.USERNAME_EXISTED,
                };
            }

            var newAccount = new Account { Username = signUpDto.Username, Password = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password) };
            await _accountRepo.AddAccount(newAccount);

            var newCustomer = new Customer
            {
                FullName = signUpDto.FullName.CapitalizeAllWords(),
                AccountId = newAccount.AccountId,
                Avatar = _configuration["Application:DefaultUserAvatar"],
            };
            await _customerRepo.AddCustomer(newCustomer);

            return new ServiceResponse<Customer>
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.SIGN_UP_SUCCESSFULLY,
                Data = newCustomer,
                AccessToken = _jwtService.GenerateAccessToken(newCustomer!),
                RefreshToken = _jwtService.GenerateRefreshToken(newAccount),
            };
        }

        public async Task<ServiceResponse> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            if (_jwtService.VerifyRefreshToken(refreshTokenDto.RefreshToken, out var principal))
            {
                var accountId = principal!.FindFirst(ClaimTypes.Name)!.Value;

                var customer = await _customerRepo.GetCustomerByAccountId(int.Parse(accountId));
                if (customer != null)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.OK,
                        Success = true,
                        Message = SuccessMessage.REFRESH_TOKEN_SUCCESSFULLY,
                        AccessToken = _jwtService.GenerateAccessToken(customer),
                    };
                }

                var staff = await _staffRepo.GetStaffByAccountId(int.Parse(accountId));
                if (staff != null)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.OK,
                        Success = true,
                        Message = SuccessMessage.REFRESH_TOKEN_SUCCESSFULLY,
                        AccessToken = _jwtService.GenerateAccessToken(staff, staff.RoleId),
                    };
                }

                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS,
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS,
                };
            }
        }

        public async Task<ServiceResponse> ChangePassword(ChangePasswordDto changePasswordDto, int authUserId, int? authRoleId)
        {
            var targetAccount = await _accountRepo.GetAccountByUserId(authUserId, authRoleId == null);
            if (targetAccount == null || !BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, targetAccount.Password))
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INCORRECT_PASSWORD,
                };
            }

            targetAccount.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _accountRepo.UpdateAccount(targetAccount);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.CHANGE_PASSWORD_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var existedCustomer = await _customerRepo.GetCustomerByEmail(forgotPasswordDto.Email);
            if (existedCustomer == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            await _mailerService.SendResetPasswordEmail(
                forgotPasswordDto.Email,
                existedCustomer.FullName,
                $"{_configuration["Application:ClientUrl"]}/auth?type=reset&token={_jwtService.GenerateResetPasswordToken(existedCustomer)}"
            );

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.RESET_PASSWORD_EMAIL_SENT,
            };
        }

        public async Task<ServiceResponse> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (_jwtService.VerifyResetPasswordToken(resetPasswordDto.ResetPasswordToken, out var principal))
            {
                var email = principal!.FindFirst(ClaimTypes.Email)!.Value;

                var account = await _accountRepo.GetCustomerAccountByEmail(email);
                if (account == null)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.UNAUTHORIZED,
                        Success = false,
                        Message = ErrorMessage.INVALID_CREDENTIALS,
                    };
                }

                account.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
                await _accountRepo.UpdateAccount(account);

                return new ServiceResponse
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.RESET_PASSWORD_SUCCESSFULLY,
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS,
                };
            }
        }

        public async Task<ServiceResponse<Customer>> GoogleAuthentication(GoogleAuthDto googleAuthDto)
        {
            var googleUserInfo = await FetchGoogleUserInfoAsync(googleAuthDto.GoogleAccessToken);
            if (googleUserInfo == null || !googleUserInfo.EmailVerified)
            {
                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.GOOGLE_AUTH_FAILED,
                };
            }

            var existedAccount = await _accountRepo.GetCustomerAccountByEmail(googleUserInfo.Email);
            if (existedAccount == null)
            {
                string randomUsername = RandomStringGenerator.GenerateRandomString(16);
                string randomPassword = RandomStringGenerator.GenerateRandomString(16);

                var newAccount = new Account { Username = randomUsername, Password = BCrypt.Net.BCrypt.HashPassword(randomPassword) };
                await _accountRepo.AddAccount(newAccount);

                var newCustomer = new Customer
                {
                    FullName = $"{googleUserInfo.LastName} {googleUserInfo.FirstName}",
                    AccountId = newAccount.AccountId,
                    Avatar = googleUserInfo.Picture ?? _configuration["Application:DefaultUserAvatar"],
                    Email = googleUserInfo.Email,
                };
                await _customerRepo.AddCustomer(newCustomer);

                await _mailerService.SendGoogleRegistrationSuccessEmail(
                    googleUserInfo.Email,
                    newCustomer.FullName,
                    randomUsername,
                    randomPassword,
                    $"{_configuration["Application:ClientUrl"]}/profile/change-password"
                );

                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.CREATED,
                    Success = true,
                    Message = SuccessMessage.GOOGLE_AUTH_SUCCESSFULLY,
                    Data = newCustomer,
                    AccessToken = _jwtService.GenerateAccessToken(newCustomer),
                    RefreshToken = _jwtService.GenerateRefreshToken(newAccount),
                };
            }
            else
            {
                var customerData = await _customerRepo.GetCustomerByAccountId(existedAccount.AccountId);

                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.GOOGLE_AUTH_SUCCESSFULLY,
                    Data = customerData,
                    AccessToken = _jwtService.GenerateAccessToken(customerData!),
                    RefreshToken = _jwtService.GenerateRefreshToken(existedAccount),
                };
            }
        }

        private async Task<GoogleUserInfoDto?> FetchGoogleUserInfoAsync(string googleAccessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", googleAccessToken);

            var response = await httpClient.GetAsync(_configuration["GoogleApi:OAuthEndPoint"]);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GoogleUserInfoDto>(json);
        }

        public async Task<ServiceResponse> CustomerDeactivateAccount(int customerId)
        {
            var targetAccount = await _accountRepo.GetAccountByUserId(customerId, true);
            if (targetAccount == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            targetAccount.IsActive = false;
            await _accountRepo.UpdateAccount(targetAccount);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DEACTIVATE_ACCOUNT_SUCCESSFULLY,
            };
        }
    }
}
