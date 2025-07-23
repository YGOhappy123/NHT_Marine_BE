using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<(List<Category>, int)> GetAllCategories(BaseQueryObject queryObject);
    }
}
