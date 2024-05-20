using System.ComponentModel.DataAnnotations;

namespace BCBlog.Client.Models
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 0)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<BlogPostDTO> BlogPosts { get; set; } = new HashSet<BlogPostDTO>();
    }
}
