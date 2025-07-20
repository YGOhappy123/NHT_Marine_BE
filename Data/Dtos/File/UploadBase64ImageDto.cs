using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.File
{
    public class UploadBase64ImageDto
    {
        [Required]
        public required string Base64Image { get; set; }
    }
}
