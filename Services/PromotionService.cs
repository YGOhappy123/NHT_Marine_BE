using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepo;
        private readonly ICouponRepository _couponRepo;
        private readonly IRoleRepository _roleRepo;

        public PromotionService(IPromotionRepository promotionRepo, ICouponRepository couponRepo, IRoleRepository roleRepo)
        {
            _promotionRepo = promotionRepo;
            _couponRepo = couponRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<List<PromotionDto>>> GetAllPromotions(BaseQueryObject queryObject)
        {
            var (promotions, total) = await _promotionRepo.GetAllPromotions(queryObject);
            var mappedPromotions = promotions.Select(p => p.ToPromotionDto()).ToList();

            return new ServiceResponse<List<PromotionDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedPromotions,
                Total = total,
                Took = promotions.Count,
            };
        }

        public async Task<ServiceResponse> AddNewPromotion(CreateUpdatePromotionDto createDto, int authRoleId)
        {
            var hasAddPromotionPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_PROMOTION.ToString());
            if (!hasAddPromotionPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var promotionWithSameName = await _promotionRepo.GetPromotionByName(createDto.Name);
            if (promotionWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.PROMOTION_EXISTED,
                };
            }

            var newPromotion = new Promotion
            {
                Name = createDto.Name.CapitalizeAllWords(),
                DiscountRate = createDto.DiscountRate,
                StartDate = TimestampHandler.GetStartOfTimeByType(createDto.StartDate, "daily"),
                EndDate = TimestampHandler.GetEndOfTimeByType(createDto.EndDate, "daily"),
                Products = [],
                CreatedBy = authRoleId,
            };

            foreach (var product in createDto.Products.Distinct())
            {
                newPromotion.Products.Add(new ProductPromotion { ProductId = product });
            }

            await _promotionRepo.AddPromotion(newPromotion);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_PROMOTION_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdatePromotion(CreateUpdatePromotionDto updateDto, int targetPromotionId, int authRoleId)
        {
            var hasUpdatePromotionPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_PROMOTION.ToString());
            if (!hasUpdatePromotionPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetPromotion = await _promotionRepo.GetPromotionById(targetPromotionId);
            if (targetPromotion == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PROMOTION_NOT_FOUND,
                };
            }

            var promotionWithSameName = await _promotionRepo.GetPromotionByName(updateDto.Name);
            if (promotionWithSameName != null && promotionWithSameName.PromotionId != targetPromotionId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.PROMOTION_EXISTED,
                };
            }

            targetPromotion.Name = updateDto.Name.Trim().CapitalizeAllWords();
            targetPromotion.DiscountRate = updateDto.DiscountRate;
            targetPromotion.StartDate = TimestampHandler.GetStartOfTimeByType(updateDto.StartDate, "daily");
            targetPromotion.EndDate = TimestampHandler.GetEndOfTimeByType(updateDto.EndDate, "daily");

            // Clear existing products and add new ones
            targetPromotion.Products.Clear();
            foreach (var productId in updateDto.Products.Distinct())
            {
                targetPromotion.Products.Add(new ProductPromotion { ProductId = productId });
            }

            await _promotionRepo.UpdatePromotion(targetPromotion);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_PROMOTION_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<Coupon>>> GetAllCoupons(BaseQueryObject queryObject)
        {
            var (coupons, total) = await _couponRepo.GetAllCoupons(queryObject);

            return new ServiceResponse<List<Coupon>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = coupons,
                Total = total,
                Took = coupons.Count,
            };
        }

        public async Task<ServiceResponse> DisablePromotion(int targetPromotionId, int authRoleId)
        {
            var hasDisablePromotionPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DISABLE_PROMOTION.ToString());
            if (!hasDisablePromotionPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetPromotion = await _promotionRepo.GetPromotionById(targetPromotionId);
            if (targetPromotion == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PROMOTION_NOT_FOUND,
                };
            }

            targetPromotion.IsActive = false;
            await _promotionRepo.UpdatePromotion(targetPromotion);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DISABLE_PROMOTION_SUCCESSFULLY,
            };
        }
    }
}
