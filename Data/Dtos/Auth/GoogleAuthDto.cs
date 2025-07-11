using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class GoogleAuthDto
    {
        [Required]
        public string GoogleAccessToken { get; set; } = string.Empty;
    }
}
