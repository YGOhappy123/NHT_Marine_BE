using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StatusId { get; set; }
    }
}
