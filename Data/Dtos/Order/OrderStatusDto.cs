namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class OrderStatusDto
    {
        public int StatusId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsDefaultState { get; set; } = false;

        public bool IsAccounted { get; set; } = false;

        public bool IsUnfulfilled { get; set; } = false;
    }
}
