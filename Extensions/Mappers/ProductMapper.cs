using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Extensions.Mappers
{
    public static class ProductMapper
    {
        public static CategoryDto ToCategoryDto(this Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                CreatedBy = category.CreatedBy,
                ParentId = category.ParentId,
                CreatedByStaff = category.CreatedByStaff?.ToStaffDto(),
            };
        }

        public static RootProductDto ToRootProductDto(this RootProduct product)
        {
            var currentTime = TimestampHandler.GetNow();

            return new RootProductDto
            {
                RootProductId = product.RootProductId,
                CategoryId = product.CategoryId,
                Category = product.Category?.ToCategoryDto(),
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                CreatedByStaff = product.CreatedByStaff?.ToStaffDto(),
                Variants = product
                    .Variants?.Select(variant => new ProductVariantDto
                    {
                        VariantId = variant.VariantId,
                        Name = variant.Name,
                        IsAdjustable = variant.IsAdjustable,
                        Options = variant
                            .Options?.Select(option => new VariantOptionDto { OptionId = option.OptionId, Value = option.Value })
                            .ToList(),
                    })
                    .ToList(),
                ProductItems = product
                    .ProductItems?.Select(item => new ProductItemDto
                    {
                        ProductItemId = item.ProductItemId,
                        ImageUrl = item.ImageUrl,
                        Price = item.Price,
                        PackingGuide = item.PackingGuide,
                        Attributes = item.Attributes?.Select(attr => new ProductAttributeDto { OptionId = attr.OptionId }).ToList(),
                        Stock = item.Inventories?.Sum(inventory => inventory.Quantity) ?? 0,
                    })
                    .ToList(),
                Promotions =
                    product
                        .Promotions?.Where(pp =>
                            pp.Promotion != null
                            && pp.Promotion.StartDate <= currentTime
                            && pp.Promotion.EndDate >= currentTime
                            && pp.Promotion.IsActive == true
                        )
                        .Select(pp => pp.Promotion!.ToPromotionDto())
                        .OrderByDescending(p => p.StartDate)
                        .ThenByDescending(p => p.CreatedAt)
                        .ToList() ?? new List<PromotionDto>(),
            };
        }
    }
}
