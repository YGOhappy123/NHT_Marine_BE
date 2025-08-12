namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class StatusTransitionGroupDto
    {
        public int? FromStatusId { get; set; }
        public OrderStatusDto? FromStatus { get; set; }
        public List<StatusTransitionDataDto>? Transitions { get; set; } = [];
    }
}
