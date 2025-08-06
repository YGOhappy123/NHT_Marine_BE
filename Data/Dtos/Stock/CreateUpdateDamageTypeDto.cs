using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class CreateUpdateDamageTypeDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
