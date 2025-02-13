using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly TabloidDbContext _context;

    public TagController(TabloidDbContext context)
    {
        _context = context;
    }

    //Endpoint to get all Tags and order them alphabetically
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Get()
    {
        var tags = _context
            .Tags.Include(t => t.PostTags)
            .ThenInclude(pt => pt.Post)
            .ToList()
            .OrderBy(t => t.Name)
            .ToList();
        var tagsDTO = tags.Select(t => new TagDTO
            {
                Id = t.Id,
                Name = t.Name,
                Posts = t
                    .PostTags.Select(pt => new PostDTO
                    {
                        Id = pt.Post.Id,
                        Title = pt.Post.Title,
                        SubTitle = pt.Post.SubTitle,
                        CategoryId = pt.Post.CategoryId,
                        Content = pt.Post.Content,
                        PublishingDate = pt.Post.PublishingDate,
                        HeaderImage = pt.Post.HeaderImage,
                        ReadTime = pt.Post.ReadTime,
                        AuthorId = pt.Post.AuthorId,
                        IsApproved = pt.Post.IsApproved,
                    })
                    .ToList(),
            })
            .ToList();
        return Ok(tagsDTO);
    }

    //Endpoint to get a single Tag by Id for the POST method
    //This endpoint will not return the Posts associated with the Tag
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetById(int id)
    {
        var tag = _context.Tags.SingleOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return NotFound();
        }
        var tagDTO = new TagDTO { Id = tag.Id, Name = tag.Name };
        return Ok(tagDTO);
    }

    //Endpoint to add a new Tag
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Post([FromBody] TagDTO tagDTO)
    {
        var tag = new Tag { Name = tagDTO.Name };
        _context.Tags.Add(tag);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tagDTO);
    }

    //Endpoint to update a Tag
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Put(int id, [FromBody] TagDTO tagDTO)
    {
        var tag = _context.Tags.SingleOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return NotFound();
        }
        tag.Name = tagDTO.Name;
        _context.SaveChanges();
        return NoContent();
    }

    //Endpoint to delete a Tag
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var tag = _context.Tags.SingleOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return NotFound();
        }
        _context.Tags.Remove(tag);
        _context.SaveChanges();
        return NoContent();
    }
}
