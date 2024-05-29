using BCBlog.Client.Models;

namespace BCBlog.Client.Services.Interfaces
{
    public interface IBlogPostDTOService
    {
        Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPost);

        Task<PagedList<BlogPostDTO>> GetPublishedBlogPostsAsync(int page, int pageSize);
        Task<PagedList<BlogPostDTO>> GetPostsByCategoryId(int categoryId, int page, int pageSize);
        Task UpdateBlogPostAsync(int blogPostId, BlogPostDTO blogPost);

        Task<BlogPostDTO?> GetBlogPostByIdAsync(int blogPostId);
        Task<PagedList<BlogPostDTO>> SearchBlogPostsAsync(string query, int page, int pageSize);
        Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug);
        Task<IEnumerable<BlogPostDTO>> GetBlogPostsAsync();
        Task PublishBlogPostAsync(int blogPostId);
        Task UnpublishBlogPostAsync(int blogPostId);
        Task SoftDeleteBlogPostAsync(int blogPostId);

        Task<TagDTO?> GetTagByIdAsync(int tagId);
        Task<PagedList<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize);
    }
}
