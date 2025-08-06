using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IPromotionService
    {
        Task<ServiceResponse<List<PromotionDto>>> GetAllPromotions(BaseQueryObject queryObject);
        Task<ServiceResponse> AddNewPromotion(CreateUpdatePromotionDto createDto, int authRoleId);
        Task<ServiceResponse<List<Coupon>>> GetAllCoupons(BaseQueryObject queryObject);
    }
}
