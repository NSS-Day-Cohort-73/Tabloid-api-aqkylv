using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private TabloidDbContext _context;

    public CommentController(TabloidDbContext context)
    {
        _context = context;
    }

    //GET all comments
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var comments = _context.Comments.Include(c => c.Author);
        var commentsDTO = comments.Select(c => new CommentDTO
        {
            Id = c.Id,
            AuthorId = c.AuthorId,
            Author = new UserProfileDTO
            {
                Id = c.Author.Id,
                FullName = c.Author.FullName,
                UserName = c.Author.UserName,
            },
            PostId = c.PostId,
            Subject = c.Subject,
            Content = c.Content,
            CreationDate = c.CreationDate,
        });
        return Ok(commentsDTO);
    }

    //GET a single comment
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        var comment = _context.Comments.SingleOrDefault(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        var commentDTO = new CommentDTO
        {
            Id = comment.Id,
            AuthorId = comment.AuthorId,
            Author = new UserProfileDTO
            {
                Id = comment.Author.Id,
                FullName = comment.Author.FullName,
                UserName = comment.Author.UserName,
            },
            PostId = comment.PostId,
            Subject = comment.Subject,
            Content = comment.Content,
            CreationDate = comment.CreationDate,
        };
        return Ok(commentDTO);
    }

    //POST a new comment
    [HttpPost]
    [Authorize]
    public IActionResult Post([FromBody] CommentDTO commentDTO)
    {
        var comment = new Comment
        {
            AuthorId = commentDTO.AuthorId,
            PostId = commentDTO.PostId,
            Subject = commentDTO.Subject,
            Content = commentDTO.Content,
            CreationDate = DateTime.Now,
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, commentDTO);
    }

    //Update a comment
    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Put(int id, [FromBody] CommentDTO commentDTO)
    {
        var comment = _context.Comments.SingleOrDefault(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        comment.Subject = commentDTO.Subject;
        comment.Content = commentDTO.Content;
        _context.SaveChanges();
        return NoContent();
    }

    //DELETE a comment
    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var comment = _context.Comments.SingleOrDefault(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        _context.Comments.Remove(comment);
        _context.SaveChanges();
        return NoContent();
    }
}
