using BCBlog.Client.Models;
using BCBlog.Client.Services.Interfaces;
using BCBlog.Helpers;
using BCBlog.Models;
using BCBlog.Services.Interfaces;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace BCBlog.Services
{
    public class BlogPostDTOService : IBlogPostDTOService
    {
        private readonly IBlogPostRepository _repository;

        public BlogPostDTOService(IBlogPostRepository repository)
        {
            _repository = repository;
        }

        public async Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPostDTO)
        {
            BlogPost newPost = new BlogPost()
            {
                Title = blogPostDTO.Title,
                Abstract = blogPostDTO.Abstract,
                Content = blogPostDTO.Content,
                IsPublished = blogPostDTO.IsPublished,
                IsDeleted = false,
                CategoryId = blogPostDTO.CategoryId,
                Created = DateTimeOffset.Now,
            };

            if (blogPostDTO.ImageUrl?.StartsWith("data:") == true)
            {
                try
                {
                    newPost.Image = UploadHelper.GetImageUpload(blogPostDTO.ImageUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            

            BlogPost createdPost = await _repository.CreateBlogPostAsync(newPost);

            IEnumerable<string> tagNames = blogPostDTO.Tags.Select(t => t.Name!);
            await _repository.AddTagsToBlogPostAsync(newPost.Id, tagNames);
            return createdPost.ToDTO();
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(int blogPostId)
        {
            BlogPost blogpost = await _repository.GetBlogPostByIdAsync(blogPostId);
            return blogpost?.ToDTO();
        }

        public async Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug)
        {
            BlogPost blogpost = (await _repository.GetBlogPostBySlugAsync(slug))!;
            return blogpost?.ToDTO();
        }
        public async Task<IEnumerable<BlogPostDTO>> GetBlogPostsAsync()
        {
            IEnumerable<BlogPost> blogPosts = await _repository.GetBlogPostsAsync();

            IEnumerable<BlogPostDTO> blogPostDTOs = blogPosts.Select(bp => bp.ToDTO());

            return blogPostDTOs;
        }

        public async Task<PagedList<BlogPostDTO>> GetPublishedBlogPostsAsync(int page, int pageSize)
        {

            PagedList<BlogPost> blogPosts = await _repository.GetPublishedBlogPostsAsync(page, pageSize);

            List<BlogPostDTO> blogPostDTOs = new List<BlogPostDTO>();

           

            foreach (BlogPost bp in blogPosts.Data)
            {
                BlogPostDTO dto = bp.ToDTO();
                blogPostDTOs.Add(dto);
            }

             PagedList<BlogPostDTO> blogpages = new PagedList<BlogPostDTO>();
                blogpages.Page = blogPosts.Page;
                blogpages.TotalItems = blogPosts.TotalItems;
                blogpages.TotalPages = blogPosts.TotalPages;
                blogpages.Data = blogPostDTOs;
            

            return blogpages;
        }

        public async Task PublishBlogPostAsync(int blogPostId)
        {
            await _repository.PublishBlogPostAsync(blogPostId);
        }

        public async Task UnpublishBlogPostAsync(int blogPostId)
        {
            await _repository.UnpublishBlogPostAsync(blogPostId);
        }

        public async Task SoftDeleteBlogPostAsync(int blogPostId)
        {
            await _repository.SoftDeleteBlogPostAsync(blogPostId);
        }

        public async Task UpdateBlogPostAsync(int blogPostId, BlogPostDTO blogPost)
        {
            await _repository.RemoveTagsFromBlogPostAsync(blogPost.Id);

            BlogPost? blogPostToUpdate = await _repository.GetBlogPostByIdAsync(blogPostId);

            if (blogPostToUpdate is not null)
            {
                blogPostToUpdate.Title = blogPost.Title ;
                blogPostToUpdate.Abstract = blogPost.Abstract;
                blogPostToUpdate.Content = blogPost.Content;
                blogPostToUpdate.IsPublished = blogPost.IsPublished;
                blogPostToUpdate.CategoryId = blogPost.CategoryId;
                blogPostToUpdate.IsDeleted = blogPost.IsDeleted;
                blogPostToUpdate.Updated = DateTimeOffset.Now;

                if (blogPost.ImageUrl!.StartsWith("data:"))
                {
                    blogPostToUpdate.Image = UploadHelper.GetImageUpload(blogPost.ImageUrl);
                }
                else
                {
                    blogPostToUpdate.Image = null;
                }

                await _repository.UpdateBlogPostAsync(blogPostToUpdate);

                IEnumerable<string> tagNames = blogPost.Tags.Select(t => t.Name!);

                await _repository.AddTagsToBlogPostAsync(blogPostToUpdate.Id, tagNames);

            }

        }
    }
}
