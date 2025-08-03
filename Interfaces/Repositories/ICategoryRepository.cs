using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<(List<Category>, int)> GetAllCategories(BaseQueryObject queryObject);
        Task<Category?> GetCategoryById(int categoryId);
        Task<Category?> GetCategoryByName(string categoryName);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task<bool> IsCategoryDeletable(int categoryId);
        Task DeleteCategory(Category category);
    }
}
