using BCBlog.Client.Models;
using BCBlog.Client.Services.Interfaces;
using BCBlog.Data;
using BCBlog.Helpers.Extensions;
using BCBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BCBlog.Controllers
{
    [Route("api/[controller]")] // api/comments
    [ApiController]
    public class CommentsController : ControllerBase
    {

        private readonly ICommentDTOService _commentService; //private read only fields

        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ICommentDTOService commentService, UserManager<ApplicationUser> userManager) //dependency injection
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        //get comment by ID - anyone'
        [HttpGet("{commentId}")]
        public async Task<ActionResult<CommentDTO?>> GetCommentById([FromRoute] int commentId)
        {

            try
            {
                CommentDTO? comment = await _commentService.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(comment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        //getcommentsbyBlogpostID GET - anyone
        //[AllowAnonymous] can make it so just this method can be used by anyone if controller is fully authorized

        [HttpGet] // GET: api/comments?blogpostId=1
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsForBlogPost([FromQuery] int blogpostId)
        {
            try
            {
                IEnumerable<CommentDTO> comments = await _commentService.GetBlogPostCommentsAsync(blogpostId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        //POST create a comment - logged in "api/comments"

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> CreateComment([FromBody] CommentDTO newComment)
        {
            string userId = _userManager.GetUserId(User)!;
            newComment.AuthorId = userId;

            CommentDTO comment = await _commentService.CreateCommentAsync(newComment);

            return Ok(comment);

        }


        [HttpPut("{commentId:int}")]
        public async Task<ActionResult> UpdateComment([FromRoute] int commentId, [FromBody] CommentDTO commentDTO)
        {

            if(commentDTO.Id != commentId)
            {
                return BadRequest();
            }

            string userId = _userManager.GetUserId(User)!;
            bool inAuthorRole = User.IsInRole("Author");
            bool inModeratorRole = User.IsInRole("Moderator");

            CommentDTO? comment = await _commentService.GetCommentByIdAsync(commentId);

            if (inAuthorRole || inModeratorRole || comment?.AuthorId == userId)
            {
                await _commentService.UpdateCommentAsync(commentDTO);
                return Ok();
            }
            else 
            { 
                return BadRequest();
            }

        }


        //Delete comment - logged in (comment belongs to them)
        [HttpDelete("{commentId:int}")] // DELETE: api/comments/5
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            string userId = _userManager.GetUserId(User)!;
            bool inAuthorRole = User.IsInRole("Author");
            bool inModeratorRole = User.IsInRole("Moderator");

            CommentDTO? comment = await _commentService.GetCommentByIdAsync(commentId);

            if (inAuthorRole || inModeratorRole || comment?.AuthorId == userId)
            {
                await _commentService.DeleteCommentAsync(commentId);
                return NoContent();
            }
            else
            {
                return NotFound();
            }


        }
    }
}
