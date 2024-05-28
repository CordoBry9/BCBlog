using BCBlog.Client.Models;
using System.Xml.Linq;

namespace BCBlog.Client.Services.Interfaces
{
    public interface ICommentDTOService
    {
        Task<CommentDTO> CreateCommentAsync(CommentDTO commentDTO);
        Task DeleteCommentAsync(int commentId);
        Task<IEnumerable<CommentDTO>> GetBlogPostCommentsAsync(int blogpostId);
        Task UpdateCommentAsync(CommentDTO commentDTO);

        Task<CommentDTO?> GetCommentByIdAsync(int commentId);
    }
}
