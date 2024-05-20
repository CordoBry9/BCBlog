using BCBlog.Client.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace BCBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

     
    }
    public static class TagExtensions
    {
        public static TagDTO ToDTO(this Tag tag)
        {
            TagDTO dto = new TagDTO()
            {
                Id = tag.Id,
                Name = tag.Name,
            };

            foreach (BlogPost blogpost in tag.BlogPosts)
            {
                tag.BlogPosts.Clear();
                BlogPostDTO blogpostDTO = blogpost.ToDTO();
                dto.BlogPosts.Add(blogpostDTO);

            }
            return dto;
        }
    }

}

