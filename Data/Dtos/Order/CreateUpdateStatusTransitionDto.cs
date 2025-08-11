using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class CreateUpdateStatusTransitionDto
    {
        [Required]
        public int FromStatusId { get; set; }

        [Required]
        public int ToStatusId { get; set; }

        [Required]
        public string TransitionLabel { get; set; } = string.Empty;
    }
}
