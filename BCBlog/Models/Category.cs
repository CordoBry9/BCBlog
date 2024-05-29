using BCBlog.Client.Models;
using BCBlog.Helpers;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BCBlog.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 0)]
        public string? Description { get; set; }
        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image { get; set; }

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();


    }
    public static class CategoryExtensions
    {
        public static CategoryDTO ToDTO(this Category category)
        {
            CategoryDTO dto = new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageId.HasValue ? $"api/Uploads/{category.ImageId}" : UploadHelper.DefaultCategoryPicture,

            };

            foreach (BlogPost blogpost in category.BlogPosts)
            {
                
                BlogPostDTO blogpostDTO = blogpost.ToDTO();
                dto.BlogPosts.Add(blogpostDTO);

            }
            return dto;
        }
    }

}
