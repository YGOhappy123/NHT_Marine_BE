using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICouponRepository
    {
        Task<(List<Coupon>, int)> GetAllCoupons(BaseQueryObject queryObject);
        Task AddCoupon(Coupon coupon);
        Task UpdateCoupon(Coupon coupon);
    }
}
