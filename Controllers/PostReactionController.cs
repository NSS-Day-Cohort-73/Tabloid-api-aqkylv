using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostReactionController : ControllerBase
{
    private readonly TabloidDbContext _dbContext;

    public PostReactionController(TabloidDbContext context)
    {
        _dbContext = context;
    }

    // GET: api/PostReaction?postId=3
    [HttpGet]
    public IActionResult GetPostReactions([FromQuery] int postId)
    {
        var reactions = _dbContext
            .PostReactions.Include(pr => pr.Reaction)
            .Include(pr => pr.UserProfile)
            .Where(pr => pr.PostId == postId)
            .Select(pr => new PostReactionDTO
            {
                Id = pr.Id,
                PostId = pr.PostId,
                UserProfileId = pr.UserProfileId,
                UserProfile = new UserProfileDTO
                {
                    Id = pr.UserProfile.Id,
                    FullName = pr.UserProfile.FullName,
                },
                ReactionId = pr.ReactionId,
                Reaction = new ReactionDTO
                {
                    Id = pr.Reaction.Id,
                    Name = pr.Reaction.Name,
                    Icon = pr.Reaction.Icon,
                },
            })
            .ToList();

        return Ok(reactions);
    }

    // POST: api/PostReaction
    [HttpPost]
    public IActionResult PostPostReaction(PostReactionDTO postReactionDTO)
    {
        var existingReaction = _dbContext.PostReactions.FirstOrDefault(pr =>
            pr.PostId == postReactionDTO.PostId && pr.UserProfileId == postReactionDTO.UserProfileId
        );

        if (existingReaction != null)
        {
            return BadRequest("User has already reacted to this post.");
        }

        var postReaction = new PostReaction
        {
            PostId = postReactionDTO.PostId,
            UserProfileId = postReactionDTO.UserProfileId,
            ReactionId = postReactionDTO.ReactionId,
        };

        _dbContext.PostReactions.Add(postReaction);
        _dbContext.SaveChanges();

        return CreatedAtAction(
            nameof(GetPostReactions),
            new { postId = postReaction.PostId },
            postReaction
        );
    }

    // DELETE: api/PostReaction?postId=3&userProfileId=5&reactionId=2
    [HttpDelete]
    public IActionResult DeletePostReaction(
        [FromQuery] int postId,
        [FromQuery] int userProfileId,
        [FromQuery] int reactionId
    )
    {
        var postReaction = _dbContext.PostReactions.FirstOrDefault(pr =>
            pr.PostId == postId && pr.UserProfileId == userProfileId && pr.ReactionId == reactionId
        );

        if (postReaction == null)
        {
            return NotFound("Reaction not found.");
        }

        _dbContext.PostReactions.Remove(postReaction);
        _dbContext.SaveChanges();

        return NoContent();
    }
}
