using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<(List<RootProduct>, int)> GetAllProducts(BaseQueryObject queryObject);
        Task<RootProduct?> GetProductById(int productId);
        Task<RootProduct?> GetProductByName(string productName);
        Task AddProduct(RootProduct product);
        Task UpdateProduct(RootProduct product);
        Task<bool> IsProductDeletable(int productId);
        Task DeleteProductById(int productId);
        Task<ProductItem?> GetProductItemById(int productItemId);
        Task<int> GetProductItemCurrentStock(int productItemId);
        Task UpdateProductItem(ProductItem productItem);
    }
}
