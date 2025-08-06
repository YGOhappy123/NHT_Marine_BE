using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
using NHT_Marine_BE.Extensions.Mappers;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Interfaces.Services;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IRoleRepository _roleRepo;

        public ProductService(IProductRepository productRepo, ICategoryRepository categoryRepo, IRoleRepository roleRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ServiceResponse<List<RootProductDto>>> GetAllProducts(BaseQueryObject queryObject)
        {
            var (products, total) = await _productRepo.GetAllProducts(queryObject);

            var mappedProducts = products.Select(rp => rp.ToRootProductDto()).ToList();
            foreach (var item in mappedProducts)
            {
                foreach (var _item in item.ProductItems ?? [])
                {
                    _item.Stock = await _productRepo.GetProductItemCurrentStock(_item.ProductItemId);
                }
            }

            return new ServiceResponse<List<RootProductDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedProducts,
                Total = total,
                Took = products.Count,
            };
        }

        public async Task<ServiceResponse<List<RootProductDto>>> SearchProductsByName(string searchTerm)
        {
            var (products, total) = await _productRepo.SearchProductsByName(searchTerm);

            var mappedProducts = products.Select(rp => rp.ToRootProductDto()).ToList();
            foreach (var item in mappedProducts)
            {
                item.DiscountRate = await CalculateDiscountRateAsync(item.RootProductId);
                foreach (var _item in item.ProductItems ?? [])
                {
                    _item.Stock = await _productRepo.GetProductItemCurrentStock(_item.ProductItemId);
                }
            }

            return new ServiceResponse<List<RootProductDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = mappedProducts,
                Total = total,
                Took = products.Count,
            };
        }

        private async Task<int> CalculateDiscountRateAsync(int productId)
        {
            var availablePromotions = await _productRepo.GetProductAvailablePromotions(productId);
            if (availablePromotions != null && availablePromotions.Count > 0)
            {
                return availablePromotions[0].DiscountRate;
            }

            return 0;
        }

        public async Task<ServiceResponse<List<DetailedProductItemDto>>> GetDetailedProductItems(List<int> productItemIds)
        {
            var (productItems, total) = await _productRepo.GetDetailedProductItems(productItemIds);

            foreach (var item in productItems)
            {
                item.DiscountRate = await CalculateDiscountRateAsync(item.RootProduct!.RootProductId);
                item.Stock = await _productRepo.GetProductItemCurrentStock(item.ProductItemId);
            }

            return new ServiceResponse<List<DetailedProductItemDto>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = productItems,
                Total = total,
                Took = productItems.Count,
            };
        }

        public async Task<ServiceResponse<RootProductDto?>> GetProductDetail(int productId)
        {
            var product = await _productRepo.GetProductById(productId);

            if (product != null)
            {
                var mappedProduct = product.ToRootProductDto();
                foreach (var item in mappedProduct.ProductItems ?? [])
                {
                    item.Stock = await _productRepo.GetProductItemCurrentStock(item.ProductItemId);
                }

                return new ServiceResponse<RootProductDto?>
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Data = mappedProduct,
                };
            }

            return new ServiceResponse<RootProductDto?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = null,
            };
        }

        public async Task<ServiceResponse> AddNewProduct(CreateProductDto createDto, int authUserId, int authRoleId)
        {
            var hasAddProductPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_PRODUCT.ToString());
            if (!hasAddProductPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var productWithSameName = await _productRepo.GetProductByName(createDto.Name);
            if (productWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_EXISTED,
                };
            }

            var newProduct = new RootProduct
            {
                Name = createDto.Name.CapitalizeAllWords(),
                Description = createDto.Description,
                CategoryId = createDto.CategoryId,
                ImageUrl = createDto.ImageUrl,
                CreatedBy = authUserId,
                Variants = [],
                ProductItems = [],
            };

            var variantOptionMap = new List<List<VariantOption>>();

            foreach (var variantDto in createDto.Variants)
            {
                var variant = new ProductVariant
                {
                    Name = variantDto.Name.CapitalizeAllWords(),
                    IsAdjustable = variantDto.IsAdjustable,
                    Options = [],
                };

                var localOptionList = new List<VariantOption>();

                foreach (var optionValue in variantDto.Options)
                {
                    var option = new VariantOption { Value = optionValue.CapitalizeAllWords() };

                    variant.Options.Add(option);
                    localOptionList.Add(option);
                }

                newProduct.Variants.Add(variant);
                variantOptionMap.Add(localOptionList);
            }

            foreach (var itemDto in createDto.ProductItems)
            {
                if (itemDto.Attributes.Count != createDto.Variants.Count)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.UNPROCESSABLE_ENTITY,
                        Success = false,
                        Message = ErrorMessage.DATA_VALIDATION_FAILED,
                    };
                }

                var item = new ProductItem
                {
                    Price = itemDto.Price,
                    ImageUrl = itemDto.ImageUrl,
                    PackingGuide = itemDto.PackingGuide,
                    Attributes = [],
                };

                for (int i = 0; i < itemDto.Attributes.Count; i++)
                {
                    int optionIndex = itemDto.Attributes[i];

                    if (optionIndex < 0 || optionIndex >= variantOptionMap[i].Count)
                    {
                        return new ServiceResponse
                        {
                            Status = ResStatusCode.UNPROCESSABLE_ENTITY,
                            Success = false,
                            Message = ErrorMessage.DATA_VALIDATION_FAILED,
                        };
                    }

                    item.Attributes.Add(new ProductAttribute { Option = variantOptionMap[i][optionIndex] });
                }

                newProduct.ProductItems.Add(item);
            }

            await _productRepo.AddProduct(newProduct);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateProductInfo(UpdateProductInfoDto updateDto, int targetProductId, int authRoleId)
        {
            var hasUpdateInfoPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_PRODUCT_INFORMATION.ToString());
            if (!hasUpdateInfoPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetProduct = await _productRepo.GetProductById(targetProductId);
            if (targetProduct == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND,
                };
            }

            var productWithSameName = await _productRepo.GetProductByName(updateDto.Name);
            if (productWithSameName != null && productWithSameName.RootProductId != targetProductId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_EXISTED,
                };
            }

            targetProduct.Name = updateDto.Name.Trim();
            targetProduct.Description = updateDto.Description.Trim();
            targetProduct.CategoryId = updateDto.CategoryId;
            targetProduct.ImageUrl = updateDto.ImageUrl;

            await _productRepo.UpdateProduct(targetProduct);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateProductItems(UpdateProductItemsDto updateDto, int targetProductId, int authRoleId)
        {
            var hasUpdateItemsPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_PRODUCT_PRICE.ToString());
            if (!hasUpdateItemsPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            foreach (var itemDto in updateDto.ProductItems)
            {
                var targetProductItem = await _productRepo.GetProductItemById(itemDto.ProductItemId);
                if (targetProductItem == null)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.NOT_FOUND,
                        Success = false,
                        Message = ErrorMessage.PRODUCT_NOT_FOUND,
                    };
                }

                if (targetProductItem.RootProductId != targetProductId)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.BAD_REQUEST,
                        Success = false,
                        Message = ErrorMessage.PRODUCT_IDS_MISMATCH,
                    };
                }

                targetProductItem.PackingGuide = itemDto.PackingGuide.Trim();
                targetProductItem.ImageUrl = itemDto.ImageUrl;
                targetProductItem.Price = itemDto.Price;

                await _productRepo.UpdateProductItem(targetProductItem);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> DeleteProduct(int targetProductId, int authRoleId)
        {
            var hasDeleteProductPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DELETE_PRODUCT.ToString());
            if (!hasDeleteProductPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetProduct = await _productRepo.GetProductById(targetProductId);
            if (targetProduct == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND,
                };
            }

            var isDeletable = await _productRepo.IsProductDeletable(targetProductId);
            if (!isDeletable)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_BEING_USED,
                };
            }

            await _productRepo.DeleteProductById(targetProductId);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<Category>>> GetAllCategories(BaseQueryObject queryObject)
        {
            var (categories, total) = await _categoryRepo.GetAllCategories(queryObject);

            return new ServiceResponse<List<Category>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = categories,
                Total = total,
                Took = categories.Count,
            };
        }
    }
}
