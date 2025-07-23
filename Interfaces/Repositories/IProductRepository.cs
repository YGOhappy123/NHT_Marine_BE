using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<(List<RootProduct>, int)> GetAllProducts(BaseQueryObject queryObject);
        Task<RootProduct?> GetProductById(int productId);
    }
}
