using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.File
{
    public class UploadImageDto
    {
        [Required]
        public required IFormFile File { get; set; }
    }
}
