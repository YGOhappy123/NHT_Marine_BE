using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IPromotionRepository
    {
        Task<(List<Promotion>, int)> GetAllPromotions(BaseQueryObject queryObject);
        Task<Promotion?> GetPromotionByName(string promotionName);
        Task<Promotion?> GetPromotionById(int promotionId);
        Task AddPromotion(Promotion promotion);
        Task UpdatePromotion(Promotion promotion);
    }
}
