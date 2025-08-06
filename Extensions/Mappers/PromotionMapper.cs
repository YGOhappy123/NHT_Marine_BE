using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class PromotionMapper
    {
        public static PromotionDto ToPromotionDto(this Promotion promotion)
        {
            return new PromotionDto
            {
                PromotionId = promotion.PromotionId,
                Name = promotion.Name,
                Description = promotion.Description,
                DiscountRate = promotion.DiscountRate,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                CreatedAt = promotion.CreatedAt,
                CreatedBy = promotion.CreatedBy,
                CreatedByStaff = promotion.CreatedByStaff?.ToStaffDto(),
            };
        }

        public static CouponDto ToCouponDto(this Coupon coupon)
        {
            return new CouponDto
            {
                CouponId = coupon.CouponId,
                Code = coupon.Code,
                Type = coupon.Type,
                Amount = coupon.Amount,
                MaxUsage = coupon.MaxUsage,
                IsActive = coupon.IsActive,
                ExpiredAt = coupon.ExpiredAt,
                CreatedAt = coupon.CreatedAt,
                CreatedBy = coupon.CreatedBy,
                CreatedByStaff = coupon.CreatedByStaff?.ToStaffDto(),
            };
        }
    }
}
