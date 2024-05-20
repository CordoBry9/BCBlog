using System.ComponentModel.DataAnnotations;

namespace BCBlog.Client.Models
{
    public class CommentDTO
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

        [Required]
        public int BlogPostId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorImageUrl { get; set; }
        public string? AuthorId { get; set; }

}
}
