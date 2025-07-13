using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Transaction
{
    public class OrderStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefaultState { get; set; } = false;
        public bool IsAccounted { get; set; } = false;
        public bool IsUnfulfilled { get; set; } = false;
    }
}
