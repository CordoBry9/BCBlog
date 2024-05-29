using BCBlog.Client.Models;
using BCBlog.Data;
using BCBlog.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BCBlog.Models
{
    public class BlogPost
    {

        private DateTimeOffset _created;
        private DateTimeOffset? _updated;
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Slug { get; set; }

        [Required]
        [Display(Name = "Abstract")]
        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Abstract { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public DateTimeOffset? Updated
        {
            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();


    }
    public static class BlogPostExtensions
    {
        public static BlogPostDTO ToDTO(this BlogPost blogpost)
        {
            BlogPostDTO dto = new BlogPostDTO()
            {
                Id = blogpost.Id,
                Title = blogpost.Title,
                Abstract = blogpost.Abstract,
                Content = blogpost.Content,
                Slug = blogpost.Slug,
                Created = blogpost.Created,
                Updated = blogpost.Updated,
                IsPublished = blogpost.IsPublished,
                IsDeleted = blogpost.IsDeleted,
                ImageUrl = blogpost.ImageId.HasValue ? $"api/Uploads/{blogpost.ImageId}" : UploadHelper.DefaultBlogPicture,
                CategoryId = blogpost.CategoryId,
            };

            if (blogpost.Category is not null)
            {
                blogpost.Category.BlogPosts = [];

                CategoryDTO categoryDTO = blogpost.Category.ToDTO();
                dto.Category = categoryDTO;
            }

            foreach (Tag tag in blogpost.Tags)
            {
                tag.BlogPosts = [];

                TagDTO tagDTO = tag.ToDTO();
                dto.Tags.Add(tagDTO);
            }

            foreach (Comment comment in blogpost.Comments)
            {
              

                CommentDTO commentDTO = comment.ToDTO();
                dto.Comments.Add(commentDTO);
            }

            return dto;
        }
    }
}
