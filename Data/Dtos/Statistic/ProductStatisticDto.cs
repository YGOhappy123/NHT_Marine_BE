namespace NHT_Marine_BE.Data.Dtos.Statistic
{
    public class ProductStatisticDto
    {
        public int ProductItemId { get; set; }
        public int TotalUnits { get; set; } = 0;
        public decimal TotalSales { get; set; } = 0;
    }
}
