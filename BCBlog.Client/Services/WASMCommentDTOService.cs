using BCBlog.Client.Models;
using BCBlog.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace BCBlog.Client.Services
{
    public class WASMCommentDTOService : ICommentDTOService
    {

        private readonly HttpClient _httpClient;

        public WASMCommentDTOService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CommentDTO> CreateCommentAsync(CommentDTO commentDTO)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/comments", commentDTO);
            response.EnsureSuccessStatusCode();

            CommentDTO? createdDTO = await response.Content.ReadFromJsonAsync<CommentDTO>();

            return createdDTO!;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/comments/{commentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<CommentDTO>> GetBlogPostCommentsAsync(int blogpostId)
        {
            IEnumerable<CommentDTO> response = (await _httpClient.GetFromJsonAsync<IEnumerable<CommentDTO>>($"api/comments?blogpostId={blogpostId}"))!;

            return response;
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
        {
            return await _httpClient.GetFromJsonAsync<CommentDTO>($"api/comments/{commentId}");
        }

        public async Task UpdateCommentAsync(CommentDTO commentDTO)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/comments/{commentDTO.Id}", commentDTO);
            response.EnsureSuccessStatusCode();
        }
    }
}
