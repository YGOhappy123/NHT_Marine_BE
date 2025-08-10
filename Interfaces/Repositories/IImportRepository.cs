using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Stock;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IImportRepository
    {
        Task<(List<ProductImport>, int)> GetAllProductImports(BaseQueryObject queryObject);
        Task<List<ProductImport>> GetAllImportsInTimeRange(DateTime startTime, DateTime endTime);
    }
}
