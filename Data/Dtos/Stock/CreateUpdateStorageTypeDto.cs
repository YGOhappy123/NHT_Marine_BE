using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class CreateUpdateStorageTypeDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
