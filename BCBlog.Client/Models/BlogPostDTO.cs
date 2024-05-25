
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BCBlog.Client.Models
{
    public class BlogPostDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;
        public int Id { get; set; }

        public string? Title { get; set; }

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
        public string? ImageUrl { get; set; }
 
        public int CategoryId { get; set; }
        public CategoryDTO? Category { get; set; }
        public ICollection<CommentDTO> Comments { get; set; } = new HashSet<CommentDTO>();
        //doesnt need to be virtual in DTO entity framework says so
        public ICollection<TagDTO> Tags { get; set; } = new HashSet<TagDTO>();
    }
}
