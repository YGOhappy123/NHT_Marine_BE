using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Repositories
{
    public class DamageReportRepository : IDamageReportRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public DamageReportRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<List<ProductDamageReport>> GetAllReportsInTimeRange(DateTime startTime, DateTime endTime)
        {
            return await _dbContext.ProductDamageReports.Where(od => od.ReportedAt >= startTime && od.ReportedAt < endTime).ToListAsync();
        }
    }
}
