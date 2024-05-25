using BCBlog.Client.Models;

namespace BCBlog.Client.Services.Interfaces
{
    public interface IBlogPostDTOService
    {
        Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPost);

        Task<PagedList<BlogPostDTO>> GetPublishedBlogPostsAsync(int page, int pageSize);

        Task UpdateBlogPostAsync(int blogPostId, BlogPostDTO blogPost);

        Task<BlogPostDTO?> GetBlogPostByIdAsync(int blogPostId);

        Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug);
        Task<IEnumerable<BlogPostDTO>> GetBlogPostsAsync();
        Task PublishBlogPostAsync(int blogPostId);
        Task UnpublishBlogPostAsync(int blogPostId);
        Task SoftDeleteBlogPostAsync(int blogPostId);
    }
}
