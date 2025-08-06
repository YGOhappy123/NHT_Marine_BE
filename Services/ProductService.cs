using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Enums;
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

        public async Task<ServiceResponse<List<RootProduct>>> GetAllProducts(BaseQueryObject queryObject)
        {
            var (products, total) = await _productRepo.GetAllProducts(queryObject);

            return new ServiceResponse<List<RootProduct>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = products,
                Total = total,
                Took = products.Count,
            };
        }

        public async Task<ServiceResponse<RootProduct?>> GetProductDetail(int productId)
        {
            var product = await _productRepo.GetProductById(productId);
            return new ServiceResponse<RootProduct?>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = product,
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

        public async Task<ServiceResponse> AddNewCategory(CreateCategoryDto createDto, int authUserId, int authRoleId)
        {
            var hasAddCategoryPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.ADD_NEW_PRODUCT_CATEGORY.ToString());
            if (!hasAddCategoryPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var categoryWithSameName = await _categoryRepo.GetCategoryByName(createDto.Name);
            if (categoryWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_EXISTED,
                };
            }

            var newCategory = new Category
            {
                Name = createDto.Name.CapitalizeAllWords(),
                CreatedBy = authUserId,
                ParentId = createDto.ParentId,
            };

            await _categoryRepo.AddCategory(newCategory);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_CATEGORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateCategory(UpdateCategoryDto updateDto, int targetCategoryId, int authRoleId)
        {
            var hasUpdateCategoryPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_PRODUCT_CATEGORY.ToString());
            if (!hasUpdateCategoryPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetCategory = await _categoryRepo.GetCategoryById(targetCategoryId);
            if (targetCategory == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_NOT_FOUND,
                };
            }

            var categoryWithSameName = await _categoryRepo.GetCategoryByName(updateDto.Name);
            if (categoryWithSameName != null && categoryWithSameName.CategoryId != targetCategoryId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_EXISTED,
                };
            }

            targetCategory.Name = updateDto.Name.Trim().CapitalizeAllWords();
            targetCategory.ParentId = updateDto.ParentId;

            await _categoryRepo.UpdateCategory(targetCategory);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_CATEGORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> DeleteCategory(int targetCategoryId, int authRoleId)
        {
            var hasDeleteCategoryPermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DELETE_PRODUCT_CATEGORY.ToString());
            if (!hasDeleteCategoryPermission)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetCategory = await _categoryRepo.GetCategoryById(targetCategoryId);
            if (targetCategory == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_NOT_FOUND,
                };
            }

            var isDeletable = await _categoryRepo.IsCategoryDeletable(targetCategoryId);
            if (!isDeletable)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_BEING_USED,
                };
            }

            await _categoryRepo.DeleteCategory(targetCategory);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.DELETE_CATEGORY_SUCCESSFULLY,
            };
        }
    }
}
