using NHT_Marine_BE.Data.Dtos.Response;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IStatisticService
    {
        Task<ServiceResponse<object>> GetSummaryStatistic(string type, int authRoleId);
        Task<ServiceResponse<object>> GetPopularStatistic(string type, int authRoleId);
        Task<ServiceResponse<object>> GetRevenuesChart(string type, int authRoleId);
        Task<ServiceResponse<object>> GetProductStatistic(int productId, int authRoleId);
    }
}
