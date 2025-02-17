using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public UploadController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string type)
    {
        // If no file, no luck
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        using var stream = file.OpenReadStream();
        // If `type` is null or empty, it’ll fall back to the default transformation
        var imageUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName, type);

        if (string.IsNullOrEmpty(imageUrl))
        {
            return StatusCode(500, "Cloudinary upload failed.");
        }

        // Return the URL as JSON
        return Ok(new { imageUrl });
    }
}
