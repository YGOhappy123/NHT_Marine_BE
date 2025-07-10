namespace NHT_Marine_BE.Data.Dtos.Response
{
    public class SuccessResponseDto
    {
        public int? Total { get; set; }
        public int? Took { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
