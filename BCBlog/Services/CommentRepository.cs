using BCBlog.Data;
using BCBlog.Models;
using BCBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BCBlog.Services
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public CommentRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();
            context.Comments.Add(comment);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return comment;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Comment? comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
            {
                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetBlogPostCommentsAsync(int blogpostId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<Comment> comments = await context.Comments.Where(b => b.BlogPostId == blogpostId).Include(b => b.Author).OrderByDescending(b => b.Created).ToListAsync();

            return comments;

        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Comment? comment = (await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId))!;

            return comment;
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            bool shouldUpdate = await context.Comments.AnyAsync(c => c.Id == comment.Id);

            if (shouldUpdate)
            {
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
            }
        }
    }
}

