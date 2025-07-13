using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Models.User;

namespace NHT_Marine_BE.Models.Transaction
{
    public class OrderStatusUpdateLog
    {
        [Key]
        public int LogId { get; set; }
        public int? OrderId { get; set; }
        public int? StatusId { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public Staff? UpdatedByStaff { get; set; }
        public Order? Order { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
