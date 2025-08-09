using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IDamageReportRepository
    {
        Task<List<ProductDamageReport>> GetAllReportsInTimeRange(DateTime startTime, DateTime endTime);
    }
}
