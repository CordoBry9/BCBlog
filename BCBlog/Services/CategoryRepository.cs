using BCBlog.Data;
using BCBlog.Models;
using BCBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BCBlog.Services
{
    public class CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICategoryRepository
    {
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Category> categories = await context.Categories.ToListAsync();

            return categories;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is not null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }

        }

        public async Task UpdateCategoryAsync(Category category)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            bool shouldEdit = await context.Categories.AnyAsync(c => c.Id == category.Id);

            if (shouldEdit)
            {

                //if theres a new immage
                //-save the image
                //-change the contact
                //-delete the old image
                ImageUpload? oldImage = null;
                if (category.Image is not null)
                {
                    context.Images.Add(category.Image); //Adds image to database table

                    oldImage = await context.Images.FirstOrDefaultAsync(i => i.Id == category.ImageId); //checks for an old image

                    category.ImageId = category.Image.Id; //fix the foreign key
                }

                //tell the context we want to change this contact
                context.Categories.Update(category);
                // save changes
                await context.SaveChangesAsync();

                if (oldImage != null)
                {
                    context.Images.Remove(oldImage);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Category? category = await context.Categories.Include(b => b.BlogPosts).FirstOrDefaultAsync(c => c.Id == categoryId);

            return category;
        }
    }
}
