using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class CreateUpdateStorageDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
    }
}
