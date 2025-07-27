using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class CreateUpdateDamageTypeDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
