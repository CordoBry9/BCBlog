
using BCBlog.Client.Models;
using System.Threading.Tasks;

namespace BCBlog.Client.Services.Interfaces
{
    public interface ICategoryDTOService
    {
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category);

        Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();

        Task DeleteCategoryAsync(int categoryId);

        Task UpdateCategoryAsync(int categoryId, CategoryDTO category);

        Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId);
    }
}