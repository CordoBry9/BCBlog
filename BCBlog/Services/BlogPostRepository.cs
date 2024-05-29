using Azure;
using BCBlog.Client.Models;
using BCBlog.Data;
using BCBlog.Helpers.Extensions;
using BCBlog.Models;
using BCBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Npgsql.Replication.TestDecoding;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BCBlog.Services
{
    public class BlogPostRepository : IBlogPostRepository
    {

        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public BlogPostRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            blogPost.Created = DateTimeOffset.Now;
            blogPost.Slug = await GenerateSlugAsync(blogPost.Title!, blogPost.Id);

            context.BlogPosts.Add(blogPost);
            await context.SaveChangesAsync();
            return blogPost;


        }
        public async Task<PagedList<BlogPost>> GetPublishedBlogPostsAsync(int page, int pageSize) //was IENumerable before
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            PagedList<BlogPost> blogPosts = await context.BlogPosts.Where(b => b.IsPublished && !b.IsDeleted).Include(b => b.Category)
                .Include(b => b.Tags).Include(b => b.Comments).OrderByDescending(b => b.Created).ToPagedListAsync(page, pageSize);

            return blogPosts;

        }

        public async Task<PagedList<BlogPost>> GetPostsByCategoryId(int categoryId, int page, int pageSize)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            PagedList<BlogPost> blogPosts = await context.BlogPosts.Where(b => b.IsPublished && !b.IsDeleted && b.CategoryId == categoryId).Include(b => b.Category)
                .Include(b => b.Tags).Include(b => b.Comments).OrderByDescending(b => b.Created).ToPagedListAsync(page, pageSize);

            return blogPosts;
        }

        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<BlogPost> blogPosts = await context.BlogPosts
                .Include(bp => bp.Category)
                .Include(bp => bp.Tags)
                .Include(bp => bp.Comments)
                .OrderByDescending(b => b.Created)
                .ToListAsync();

            return blogPosts;
        }

        public async Task<PagedList<BlogPost>> GetPostsByTagIdAsync(int tagId, int page, int pageSize)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            PagedList<BlogPost> posts = await context.BlogPosts
                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                .Include(b => b.Category)
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .Where(b => b.Tags.Any(t => t.Id == tagId) /* blog post has a tag of this ID*/)
                .OrderByDescending(b => b.Created).ToPagedListAsync(page, pageSize);
            return posts;
        }

        public async Task<Tag?> GetTagByIdAsync(int tagId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

            return tag;
        }
        public async Task<PagedList<BlogPost>> SearchBlogPostsAsync(string query, int page, int pageSize)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            string normalizedQuery = query.Trim().ToLower();
            PagedList<BlogPost> results = await context.BlogPosts
                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                .Include(b => b.Category)
                .Include(b => b.Tags)
                .Include(b => b.Comments).ThenInclude(c => c.Author)
                .Where(b => string.IsNullOrWhiteSpace(normalizedQuery)
                       || b.Title!.ToLower().Contains(normalizedQuery)
                       || b.Abstract!.ToLower().Contains(normalizedQuery)
                       || b.Content!.ToLower().Contains(normalizedQuery)
                       || b.Category!.Name!.ToLower().Contains(normalizedQuery)
                       || b.Tags.Select(t => t.Name!.ToLower()).Any(tagName => tagName.Contains(normalizedQuery))
                       || b.Comments.Any(c => c.Content!.ToLower().Contains(normalizedQuery) 
                                    || c.Author!.FirstName!.ToLower().Contains(normalizedQuery) 
                                    || c.Author!.LastName!.ToLower().Contains(normalizedQuery))

                )
                .OrderByDescending(b => b.Created)
                .ToPagedListAsync(page, pageSize);
            return results;
        }

        public async Task PublishBlogPostAsync(int blogPostId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            BlogPost? blogPost = await context.BlogPosts.FindAsync(blogPostId);

            if (blogPost != null)
            {
                blogPost.IsPublished = true;
                context.BlogPosts.Update(blogPost);
                await context.SaveChangesAsync();
            }
        }

        public async Task UnpublishBlogPostAsync(int blogPostId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            BlogPost? blogPost = await context.BlogPosts.FindAsync(blogPostId);

            if (blogPost != null)
            {
                blogPost.IsPublished = false;
                context.BlogPosts.Update(blogPost);
                await context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteBlogPostAsync(int blogPostId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            BlogPost? blogPost = await context.BlogPosts.FindAsync(blogPostId);

            if (blogPost != null)
            {
                blogPost.IsDeleted = true;
                context.BlogPosts.Update(blogPost);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateBlogPostAsync(BlogPost blogPost)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            bool shouldEdit = await context.BlogPosts.AnyAsync(bp => bp.Id == blogPost.Id);

            if (shouldEdit)
            {

                //if theres a new immage
                //-save the image
                //-change the contact
                //-delete the old image
                ImageUpload? oldImage = null;
                if (blogPost.Image is not null)
                {
                    context.Images.Add(blogPost.Image); //Adds image to database table

                    oldImage = await context.Images.FirstOrDefaultAsync(i => i.Id == blogPost.ImageId); //checks for an old image

                    blogPost.ImageId = blogPost.Image.Id; //fix the foreign key
                }
                blogPost.Updated = DateTimeOffset.Now;
                //updates the slug
                
                blogPost.Slug = await GenerateSlugAsync(blogPost.Title!, blogPost.Id);
                //tell the context we want to change this contact
                context.BlogPosts.Update(blogPost);
                // save changes
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                if (oldImage != null)
                {
                    context.Images.Remove(oldImage);
                    await context.SaveChangesAsync();
                }
            }

        } 
        public async Task<BlogPost> GetBlogPostByIdAsync(int blogPostId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();
            
            BlogPost blogPost = (await context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId))!;

            return blogPost;
        }
        public async Task AddTagsToBlogPostAsync(int blogPostId, IEnumerable<string> tagNames)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            TextInfo textInfo = new CultureInfo("end-US").TextInfo;

            BlogPost? blogPost = await context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId);

            if (blogPost != null)
            {
                foreach(string tagName in tagNames)
                {
                    Tag? existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Name!.Trim().ToLower() == tagName.Trim().ToLower());

                    if (existingTag != null)
                    {
                        blogPost.Tags.Add(existingTag);
                    }
                    else
                    {
                        string titleCaseTagName = textInfo.ToTitleCase(tagName);
                        Tag newTag = new Tag() { Name = titleCaseTagName };

                        context.Tags.Add(newTag);
                        blogPost.Tags.Add(newTag);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveTagsFromBlogPostAsync(int blogPostId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            BlogPost? blogPost = await context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId);

            if (blogPost is not null)
            {
                blogPost.Tags.Clear();
                await context.SaveChangesAsync();

                //TODO: remove any tags that have no posts???
            }

        }

        public async Task<BlogPost?> GetBlogPostBySlugAsync(string slug)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            BlogPost blogPost = (await context.BlogPosts.Where(b=>b.IsPublished == true && b.IsDeleted == false)
                .Include(b => b.Comments).ThenInclude(c => c.Author).OrderByDescending(b => b.Created)
                .Include(b => b.Tags).Include(b => b.Category).FirstOrDefaultAsync(b => b.Slug == slug))!;

            return blogPost;
        }
        
        
        private async Task<string> GenerateSlugAsync(string title, int id)
        {
            // somehow take a title and make it a slug e.g. "my first post" becomes "my-first-post"
            // somehow validate the slug e.g is it already taken by another post

            // 1) generate a slug
            if (await ValidatedSlugAsync(title, id))
            {
                return Slugify(title); //"my-first-post
            }
            else
            {
                int i = 2;
                string newTitle = $"{title} {i}"; // "my-first-post-2
                bool isValid = await ValidatedSlugAsync(newTitle, id);

                while (isValid == false)
                {
                    i++;
                    newTitle = $"{title} {i}"; // "my-first-post-3
                    isValid = await ValidatedSlugAsync(newTitle, id);
                }

                return Slugify(newTitle);
            }
            // 2) if its not valid, make a new one
            // 3) return a valid one

        }


        private async Task<bool> ValidatedSlugAsync(string title, int blogId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            string newSlug = Slugify(title);

            bool isValid = false;

            if (blogId == 0)
            {
                // this is a new ost so just check if anyone has this slug
                isValid = !await context.BlogPosts.AnyAsync(bp => bp.Slug == newSlug);
            }
            else
            {
                // this is an existing post so just check if anyone has this slug and id
                isValid = !await context.BlogPosts.AnyAsync(bp => bp.Slug == newSlug && bp.Id != blogId);
            }

            return isValid;
        }

        private static string Slugify(string title)
        {
            if (string.IsNullOrEmpty(title)) return title;

            title = title.Normalize(System.Text.NormalizationForm.FormD);
            char[] chars = title.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            string normalizedTitle = new string(chars).Normalize(System.Text.NormalizationForm.FormC).ToLower().Trim();

            string titleWithoutSymbols = Regex.Replace(normalizedTitle, @"[^A-Za-z0-9\s-]", "");
            string slug = Regex.Replace(titleWithoutSymbols, @"\s+", "-");

            return slug;
        }




        #region tags


        #endregion
    }
}
