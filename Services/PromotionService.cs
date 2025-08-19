using NHT_Marine_BE.Data.Dtos.Auth;
using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Dtos.Transaction;
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

        public async Task<ServiceResponse> AddNewCoupon(CreateCouponDto createDto, int authUserId, int authRoleId)
        {
            var hasAddCouponPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_COUPON.ToString());
            if (!hasAddCouponPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var couponWithSameName = await _couponRepo.GetCouponByName(createDto.Code);
            if (couponWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.COUPON_EXISTED,
                };
            }

            var newCoupon = new Coupon
            {
                Code = createDto.Code,
                Type = createDto.Type,
                Amount = createDto.Amount,
                MaxUsage = createDto.MaxUsage,
                IsActive = true,
                ExpiredAt = TimestampHandler.GetEndOfTimeByType(createDto.ExpiredAt, "daily"),
                CreatedBy = authUserId,
            };

            await _couponRepo.AddCoupon(newCoupon);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_COUPON_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateCoupon(UpdateCouponDto updateDto, int targetCouponId, int authRoleId)
        {
            var hasUpdateCouponPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_COUPON.ToString());
            if (!hasUpdateCouponPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetCoupon = await _couponRepo.GetCouponById(targetCouponId);
            if (targetCoupon == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.COUPON_NOT_FOUND,
                };
            }

            // Handle null checks for nullable properties
            if (!string.IsNullOrEmpty(updateDto.Code))
            {
                var couponWithSameName = await _couponRepo.GetCouponByName(updateDto.Code);
                if (couponWithSameName != null && couponWithSameName.CouponId != targetCouponId)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.CONFLICT,
                        Success = false,
                        Message = ErrorMessage.COUPON_EXISTED,
                    };
                }
                targetCoupon.Code = updateDto.Code;
            }

            // Only update properties if they have values
            if (updateDto.Type.HasValue)
                targetCoupon.Type = updateDto.Type.Value;

            if (updateDto.Amount.HasValue)
                targetCoupon.Amount = updateDto.Amount.Value;

            if (updateDto.MaxUsage.HasValue)
                targetCoupon.MaxUsage = updateDto.MaxUsage.Value;
            else
                targetCoupon.MaxUsage = null;

            if (updateDto.IsActive.HasValue)
                targetCoupon.IsActive = updateDto.IsActive.Value;

            if (updateDto.ExpiredAt.HasValue)
                targetCoupon.ExpiredAt = TimestampHandler.GetEndOfTimeByType(updateDto.ExpiredAt.Value, "daily");

            await _couponRepo.UpdateCoupon(targetCoupon);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_COUPON_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> DisableCoupon(int targetCouponId, int authRoleId)
        {
            var hasDisableCouponPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DISABLE_COUPON.ToString());
            if (!hasDisableCouponPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetCoupon = await _couponRepo.GetCouponById(targetCouponId);
            if (targetCoupon == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.COUPON_NOT_FOUND,
                };
            }

            targetCoupon.IsActive = false;
            await _couponRepo.UpdateCoupon(targetCoupon);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DISABLE_COUPON_SUCCESSFULLY,
            };
        }
    }
}
