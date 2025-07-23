using NHT_Marine_BE.Data.Dtos.Response;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<List<RootProduct>>> GetAllProducts(BaseQueryObject queryObject);
        Task<ServiceResponse<RootProduct?>> GetProductDetail(int productId);
        Task<ServiceResponse<List<Category>>> GetAllCategories(BaseQueryObject queryObject);
    }
}
