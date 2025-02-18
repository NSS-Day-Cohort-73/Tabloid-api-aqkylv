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

    // GET: api/PostTag?postId=3&tagId=5
    [HttpGet]
    public IActionResult GetPostTags([FromQuery] int postId, [FromQuery] int? tagId = null)
    {
        if (tagId.HasValue)
        {
            var postTag = _dbContext
                .PostTags.Include(pt => pt.Tag)
                .FirstOrDefault(pt => pt.PostId == postId && pt.TagId == tagId.Value);

            if (postTag == null)
            {
                return NotFound("PostTag not found.");
            }

            var postTagDTO = new PostTagDTO
            {
                PostId = postTag.PostId,
                TagId = postTag.TagId,
                Tag = new TagDTO { Id = postTag.Tag.Id, Name = postTag.Tag.Name },
            };

            return Ok(postTagDTO);
        }
        else
        {
            var query = _dbContext.PostTags.Include(pt => pt.Tag).Where(pt => pt.PostId == postId);

            var postTags = query
                .Select(pt => new PostTagDTO
                {
                    PostId = pt.PostId,
                    TagId = pt.TagId,
                    Tag = new TagDTO { Id = pt.Tag.Id, Name = pt.Tag.Name },
                })
                .ToList();

            return Ok(postTags);
        }
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
            nameof(GetPostTags),
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

    // PUT: api/PostTag?postId=3
    [HttpPut]
    public IActionResult UpdatePostTags(
        [FromQuery] int postId,
        [FromBody] List<PostTagDTO> postTags
    )
    {
        var existingPostTags = _dbContext.PostTags.Where(pt => pt.PostId == postId).ToList();

        // Find tags to remove
        var tagsToRemove = existingPostTags
            .Where(pt => !postTags.Any(dto => dto.TagId == pt.TagId))
            .ToList();

        // Find tags to add
        var tagsToAdd = postTags
            .Where(dto => !existingPostTags.Any(pt => pt.TagId == dto.TagId))
            .Select(dto => new PostTag { PostId = postId, TagId = dto.TagId })
            .ToList();

        // Remove tags
        if (tagsToRemove.Any())
        {
            _dbContext.PostTags.RemoveRange(tagsToRemove);
        }

        // Add new tags
        if (tagsToAdd.Any())
        {
            _dbContext.PostTags.AddRange(tagsToAdd);
        }

        _dbContext.SaveChanges();

        return NoContent();
    }
}
