namespace NHT_Marine_BE.Data.Queries
{
    public class BaseQueryObject
    {
        public int? Skip { get; set; }
        public int? Limit { get; set; }
        public string? Sort { get; set; } = "{\"CreatedAt\": \"DESC\"}";
        public string? Filter { get; set; } = "{}";
    }

    public class TimeRangeQueryObject
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
