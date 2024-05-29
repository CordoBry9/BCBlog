using BCBlog.Client.Models;
using BCBlog.Models;

namespace BCBlog.Services.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost);
        Task<PagedList<BlogPost>> GetPublishedBlogPostsAsync(int page, int pageSize);
        Task<PagedList<BlogPost>> GetPostsByCategoryId(int categoryId, int page, int pageSize);
        Task<BlogPost> GetBlogPostByIdAsync(int blogPostId);
        Task UpdateBlogPostAsync(BlogPost blogPost);
        Task AddTagsToBlogPostAsync(int blogPostId, IEnumerable<string> tagNames);
        Task RemoveTagsFromBlogPostAsync(int blogPostId);
        Task PublishBlogPostAsync(int blogPostId);
        Task UnpublishBlogPostAsync(int blogPostId);
        Task SoftDeleteBlogPostAsync(int blogPostId);
        //get draft posts?
        Task<PagedList<BlogPost>> SearchBlogPostsAsync(string query, int page, int pageSize);
        Task<IEnumerable<BlogPost>> GetBlogPostsAsync();
        Task<BlogPost?> GetBlogPostBySlugAsync(string slug);
        Task<Tag?> GetTagByIdAsync(int tagId);
        Task<PagedList<BlogPost>> GetPostsByTagIdAsync(int tagId, int page, int pageSize);
        
    }
}
