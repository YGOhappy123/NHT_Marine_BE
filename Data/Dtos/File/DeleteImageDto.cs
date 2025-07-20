using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.File
{
    public class DeleteImageDto
    {
        [Required]
        [Url]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
