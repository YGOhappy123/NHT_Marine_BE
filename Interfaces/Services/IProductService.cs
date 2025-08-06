using NHT_Marine_BE.Data.Dtos.Product;
using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<List<RootProductDto>>> GetAllProducts(BaseQueryObject queryObject);
        Task<ServiceResponse<List<RootProductDto>>> SearchProductsByName(string searchTerm);
        Task<ServiceResponse<List<DetailedProductItemDto>>> GetDetailedProductItems(List<int> productItemIds);
        Task<ServiceResponse<RootProductDto?>> GetProductDetail(int productId);
        Task<ServiceResponse> AddNewProduct(CreateProductDto createDto, int authUserId, int authRoleId);
        Task<ServiceResponse> UpdateProductInfo(UpdateProductInfoDto updateDto, int targetProductId, int authRoleId);
        Task<ServiceResponse> UpdateProductItems(UpdateProductItemsDto updateDto, int targetProductId, int authRoleId);
        Task<ServiceResponse> DeleteProduct(int targetProductId, int authRoleId);
        Task<ServiceResponse<List<Category>>> GetAllCategories(BaseQueryObject queryObject);
        Task<ServiceResponse> AddNewCategory(CreateCategoryDto createDto, int authUserId, int authRoleId);
        Task<ServiceResponse> UpdateCategory(UpdateCategoryDto updateDto, int targetCategoryId, int authRoleId);
        Task<ServiceResponse> DeleteCategory(int targetCategoryId, int authRoleId);
    }
}
