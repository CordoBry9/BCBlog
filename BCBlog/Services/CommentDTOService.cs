using BCBlog.Client.Models;
using BCBlog.Client.Services.Interfaces;
using BCBlog.Helpers;
using BCBlog.Models;
using BCBlog.Services.Interfaces;

namespace BCBlog.Services
{
    public class CommentDTOService(ICommentRepository repository) : ICommentDTOService
    {


        public async Task<CommentDTO> CreateCommentAsync(CommentDTO commentDTO)
        {
            Comment? comment = new Comment()
            {
                Content = commentDTO.Content,
                Created = DateTimeOffset.Now,
                BlogPostId = commentDTO.BlogPostId,
                AuthorId = commentDTO.AuthorId,
                
            };

         
                //if (commentDTO.AuthorImageUrl?.StartsWith("data:") == true)
                //{
                //    try
                //    {
                //        comment.Author.Image = UploadHelper.GetImageUpload(commentDTO.AuthorImageUrl);
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex);
                //    }
                //}
        
           

            Comment createdComment = await repository.CreateCommentAsync(comment);
            return createdComment.ToDTO();

        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await repository.DeleteCommentAsync(commentId);
        }

        // THIS MUST ONLY GET COMMENTS FOR ONE BLOG POST .Where blogpostID ???
        public async Task<IEnumerable<CommentDTO>> GetBlogPostCommentsAsync(int blogpostId)
        {
            // get comments from the repository
            IEnumerable<Comment> comments = await repository.GetBlogPostCommentsAsync(blogpostId);

            // make a list to hold the converted CommentDTOs
            List<CommentDTO> commentDTOs = new List<CommentDTO>();

            // Iterate over the comments and convert each one to a CommentDTO
            foreach (Comment comment in comments)
            {
                CommentDTO commentDTO = comment.ToDTO();
                commentDTOs.Add(commentDTO);
            }

            // Return the list of CommentDTOs as an IEnumerable<CommentDTO>
            return commentDTOs;
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
        {
            Comment comment = await repository.GetCommentByIdAsync(commentId);

            return comment.ToDTO();
        }

        public async Task UpdateCommentAsync(CommentDTO commentDTO)
        {
            Comment? updateComment = await repository.GetCommentByIdAsync(commentDTO.Id);

            if(updateComment != null)
            {
                updateComment.Updated = DateTimeOffset.Now;
                updateComment.BlogPostId = commentDTO.BlogPostId;
                updateComment.UpdateReason = commentDTO.UpdateReason;
                updateComment.Content = commentDTO.Content;

                await repository.UpdateCommentAsync(updateComment);
            }
        }
    }
}

