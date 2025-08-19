using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICouponRepository
    {
        Task<(List<Coupon>, int)> GetAllCoupons(BaseQueryObject queryObject);
        Task<Coupon?> GetCouponById(int couponId);
        Task<Coupon?> GetCouponByName(string couponName);
        Task AddCoupon(Coupon coupon);
        Task UpdateCoupon(Coupon coupon);
        Task<Coupon?> GetCouponByCode(string code);
        Task<int> CountCouponUsage(int couponId);
        Task<bool> CheckCustomerCouponUsed(int couponId, int customerId);
    }
}
