using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Models.Product;

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
    }
}
