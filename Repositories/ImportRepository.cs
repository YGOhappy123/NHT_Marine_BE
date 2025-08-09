using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ImportRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<List<ProductImport>> GetAllImportsInTimeRange(DateTime startTime, DateTime endTime)
        {
            return await _dbContext.ProductImports.Where(od => od.TrackedAt >= startTime && od.TrackedAt < endTime).ToListAsync();
        }
    }
}
