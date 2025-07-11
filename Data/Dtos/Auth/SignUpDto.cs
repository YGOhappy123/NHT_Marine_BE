using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class SignUpDto
    {
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;
    }
}
