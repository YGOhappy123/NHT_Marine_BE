using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
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

        public ProductService(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
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
