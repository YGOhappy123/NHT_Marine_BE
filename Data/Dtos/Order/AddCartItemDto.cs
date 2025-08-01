using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class AddCartItemDto
    {
        [Required]
        public int ProductItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
