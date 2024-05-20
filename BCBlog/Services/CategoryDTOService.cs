using BCBlog.Client.Models;
using BCBlog.Client.Services.Interfaces;
using BCBlog.Helpers;
using BCBlog.Models;
using BCBlog.Services.Interfaces;

namespace BCBlog.Services
{
    public class CategoryDTOService(ICategoryRepository repository) : ICategoryDTOService
    {
        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category)
        {
            Category newCategory = new Category()
            {
                Name = category.Name,
                Description = category.Description,
            };
            if (category.ImageUrl?.StartsWith("data:") == true)
            {
                newCategory.Image = UploadHelper.GetImageUpload(category.ImageUrl);
            }

            Category createdCategory = await repository.CreateCategoryAsync(newCategory);

            return createdCategory.ToDTO();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            await repository.DeleteCategoryAsync(categoryId);
           
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await repository.GetCategoriesAsync();

            IEnumerable<CategoryDTO> categoryDTOs = categories.Select(c => c.ToDTO());

            return categoryDTOs;
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId)
        {
            Category? category = await repository.GetCategoryByIdAsync(categoryId);
            return category?.ToDTO();
        }

        public async Task UpdateCategoryAsync(int categoryId, CategoryDTO category)
        {
            Category? categoryToUpdate = await repository.GetCategoryByIdAsync(categoryId);

            if (categoryToUpdate is not null)
            {
                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Description = category.Description;
                //categoryToUpdate.Image = category.ImageUrl;

                if (category.ImageUrl!.StartsWith("data:"))
                {
                    categoryToUpdate.Image = UploadHelper.GetImageUpload(category.ImageUrl);
                }
                else
                {
                    categoryToUpdate.Image = null;
                }

                categoryToUpdate.BlogPosts.Clear();

                await repository.UpdateCategoryAsync(categoryToUpdate);

            }
            
        }
    }
}
