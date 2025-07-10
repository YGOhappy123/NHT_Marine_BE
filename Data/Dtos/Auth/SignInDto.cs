using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class SignInDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
