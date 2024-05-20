using BCBlog.Client.Models;
using BCBlog.Data;
using BCBlog.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BCBlog.Models
{
    public class Comment
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;
        public int Id { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 2)]
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

        [StringLength(200)]
        public string? UpdateReason { get; set; }

        // Navigation properties
        [Required]
        public string? AuthorId { get; set; }
        public virtual ApplicationUser? Author { get; set; }

        [Required]
        public int BlogPostId { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
    }

    public static class CommentExtensions
    {
        public static CommentDTO ToDTO(this Comment comment)
        {
            CommentDTO dto = new CommentDTO()
            {
                Id = comment.Id,
                Content = comment.Content,
                Created = comment.Created,
                Updated = comment.Updated,
                UpdateReason = comment.UpdateReason,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author?.FullName,
                AuthorImageUrl = comment.Author?.ImageId.HasValue == true ? $"api/Uploads/{comment.Author.ImageId}" : UploadHelper.DefaultProfilePicture,
                BlogPostId = comment.BlogPostId
            };
            
            return dto;

        }
    }

}
