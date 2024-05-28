using BCBlog.Models;

namespace BCBlog.Services.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        // Other methods like GetCommentsByBlogPostIdAsync can be added here
        Task DeleteCommentAsync(int commentId);
        Task<IEnumerable<Comment>> GetBlogPostCommentsAsync(int blogpostId);

        Task<Comment> GetCommentByIdAsync(int commentId);

        Task UpdateCommentAsync(Comment comment);
    }

}
