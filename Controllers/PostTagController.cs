using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostTagController : ControllerBase
{
    private readonly TabloidDbContext _dbContext;

    public PostTagController(TabloidDbContext context)
    {
        _dbContext = context;
    }

    // GET: api/PostTag?postId=3
    [HttpGet]
    public IActionResult GetPostTags([FromQuery] int postId)
    {
        var query = _dbContext.PostTags.Include(pt => pt.Tag).Where(pt => pt.PostId == postId);

        var tags = query.Select(pt => new TagDTO { Id = pt.Tag.Id, Name = pt.Tag.Name }).ToList();

        return Ok(tags);
    }

    // GET: api/PostTag?postId=3&tagId=5
    [HttpGet]
    public IActionResult GetPostTagById([FromQuery] int postId, [FromQuery] int tagId)
    {
        var postTag = _dbContext
            .PostTags.Include(pt => pt.Tag)
            .FirstOrDefault(pt => pt.PostId == postId && pt.TagId == tagId);

        if (postTag == null)
        {
            return NotFound("PostTag not found.");
        }

        var tagDTO = new TagDTO { Id = postTag.Tag.Id, Name = postTag.Tag.Name };

        return Ok(tagDTO);
    }

    // POST: api/PostTag
    [HttpPost]
    public IActionResult PostPostTag(PostTagDTO postTagDTO)
    {
        var existingTag = _dbContext.PostTags.FirstOrDefault(pt =>
            pt.PostId == postTagDTO.PostId && pt.TagId == postTagDTO.TagId
        );

        if (existingTag != null)
        {
            return BadRequest("Tag already exists on post.");
        }

        var postTag = new PostTag { PostId = postTagDTO.PostId, TagId = postTagDTO.TagId };

        _dbContext.PostTags.Add(postTag);
        _dbContext.SaveChanges();

        return CreatedAtAction(
            nameof(GetPostTagById),
            new { postId = postTag.PostId, tagId = postTag.TagId },
            postTag
        );
    }

    // DELETE: api/PostTag?postId=3&tagId=5
    [HttpDelete]
    public IActionResult DeletePostTag([FromQuery] int postId, [FromQuery] int tagId)
    {
        var postTag = _dbContext.PostTags.FirstOrDefault(pt =>
            pt.PostId == postId && pt.TagId == tagId
        );

        if (postTag == null)
        {
            return NotFound("PostTag not found.");
        }

        _dbContext.PostTags.Remove(postTag);
        _dbContext.SaveChanges();

        return NoContent();
    }
}
