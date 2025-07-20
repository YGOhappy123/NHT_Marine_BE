using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.User
{
    public class UpdateStaffDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Url]
        public string Avatar { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
