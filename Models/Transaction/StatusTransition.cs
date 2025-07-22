using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Transaction
{
    public class StatusTransition
    {
        [Key]
        public int TransitionId { get; set; }
        public int? FromStatusId { get; set; }
        public int? ToStatusId { get; set; }
        public string TransitionLabel { get; set; } = string.Empty;
        public OrderStatus? FromStatus { get; set; }
        public OrderStatus? ToStatus { get; set; }
    }
}
