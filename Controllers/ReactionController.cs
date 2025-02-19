using Microsoft.AspNetCore.Mvc;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReactionController : ControllerBase
{
    private readonly TabloidDbContext _dbContext;

    public ReactionController(TabloidDbContext context)
    {
        _dbContext = context;
    }

    // GET: api/Reaction
    [HttpGet]
    public IActionResult GetReactions()
    {
        var reactions = _dbContext
            .Reactions.Select(r => new ReactionDTO
            {
                Id = r.Id,
                Name = r.Name,
                Icon = r.Icon,
            })
            .ToList();

        return Ok(reactions);
    }

    // POST: api/Reaction
    [HttpPost]
    public IActionResult PostReaction(ReactionDTO reactionDTO)
    {
        if (_dbContext.Reactions.Any(r => r.Name == reactionDTO.Name))
        {
            return BadRequest("Reaction already exists.");
        }

        var reaction = new Reaction { Name = reactionDTO.Name, Icon = reactionDTO.Icon };

        _dbContext.Reactions.Add(reaction);
        _dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetReactions), new { id = reaction.Id }, reaction);
    }

    // DELETE: api/Reaction/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteReaction(int id)
    {
        var reaction = _dbContext.Reactions.Find(id);

        if (reaction == null)
        {
            return NotFound("Reaction not found.");
        }

        _dbContext.Reactions.Remove(reaction);
        _dbContext.SaveChanges();

        return NoContent();
    }
}
