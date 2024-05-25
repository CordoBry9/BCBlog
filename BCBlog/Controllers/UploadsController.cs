using BCBlog.Client.Helpers;
using BCBlog.Data;
using BCBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BCBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UploadsController(ApplicationDbContext context) : ControllerBase
    {

        [HttpGet("{id:guid}")] 
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60)]

        public async Task<IActionResult> GetImage(Guid id)
        {
            ImageUpload? image = await context.Images.FirstOrDefaultAsync(i => i.Id == id);

            return image == null ? NotFound() : File(image.Data!, image.Type!); //turnary statement

        }

        [HttpGet("author")] //api/uploads/author
        [OutputCache(Duration = 60 * 60)]

        public async Task<IActionResult> GetAuthorImage([FromServices] IConfiguration config)
        {
            string? authorEmail = config["AdminEmail"] ?? Environment.GetEnvironmentVariable("AdminEmail");

            ApplicationUser? author = await context.Users.Include(u => u.Image).FirstOrDefaultAsync(u => u.Email == authorEmail);

            if (author?.Image is not null) //checks if both author and image are null
            {
                return File(author.Image.Data!, author.Image.Type!);    
            }
            else
            {
                string extension = ImageHelper.DefaultProfilePicture.Split('.')[^1];
                if (extension == "svg") extension = "svg+xml";

                return File(ImageHelper.DefaultProfilePicture, $"image/{extension}");
            }
        }
    }
}
