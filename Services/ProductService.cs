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

        public async Task<ServiceResponse> UpdateProductInfo(UpdateProductInfoDto updateDto, int targetProductId, int authRoleId)
        {
            var hasAddRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_PRODUCT_INFORMATION.ToString());
            if (!hasAddRolePermission)
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
            var hasAddRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.UPDATE_PRODUCT_PRICE.ToString());
            if (!hasAddRolePermission)
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
            var hasAddRolePermission = await _roleRepo.VerifyPermission(authRoleId, Permission.DELETE_PRODUCT.ToString());
            if (!hasAddRolePermission)
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
