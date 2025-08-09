using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class ChangeInventoryLocationDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int NewStorageId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ProductItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
