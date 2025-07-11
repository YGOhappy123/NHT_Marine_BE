namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IMailerService
    {
        Task SendResetPasswordEmail(string emailTo, string fullname, string resetPasswordUrl);
        Task SendGoogleRegistrationSuccessEmail(
            string emailTo,
            string fullname,
            string username,
            string password,
            string changePasswordUrl
        );
    }
}
