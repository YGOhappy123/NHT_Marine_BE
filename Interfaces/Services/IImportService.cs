using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Stock;
using NHT_Marine_BE.Data.Queries;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IImportService
    {
        Task<ServiceResponse<List<ImportDto>>> GetAllImports(BaseQueryObject queryObject);
    }
}
