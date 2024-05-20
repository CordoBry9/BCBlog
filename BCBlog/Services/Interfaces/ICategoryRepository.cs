using BCBlog.Models;

namespace BCBlog.Services.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task UpdateCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
    }
}
