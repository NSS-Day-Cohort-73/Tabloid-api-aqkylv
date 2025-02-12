using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private TabloidDbContext _dbContext;

    public PostController(TabloidDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    //[Authorize]
    public IActionResult Get()
    {
        var posts = _dbContext
            .Posts.Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.IsApproved == true)
            .Where(p => p.PublishingDate < DateTime.Now)
            .OrderByDescending(p => p.PublishingDate)
            .Select(p => new GetPostListDTO
            {
                Id = p.Id,
                Title = p.Title,
                PublishingDate = p.PublishingDate,
                IsApproved = p.IsApproved,
                Author = new UserProfileDTO { Id = p.Author.Id, FullName = p.Author.FullName },
                Category = new CategoryDTO { Id = p.Category.Id, Name = p.Category.Name },
            })
            .ToList();

        return Ok(posts);
    }

    [HttpGet("{id}")]
    //[Authorize]
    public IActionResult GetPost(int id)
    {
        var post = _dbContext
            .Posts.Include(p => p.Author)
            .Include(p => p.Category)
            .SingleOrDefault(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpPost]
    //[Authorize]
    public IActionResult Post(CreatePostDTO postDTO)
    {
        Post post = new Post
        {
            Title = postDTO.Title,
            Content = postDTO.Content,
            CategoryId = postDTO.CategoryId,
            AuthorId = postDTO.AuthorId,
            PublishingDate = DateTime.Now,
            SubTitle = postDTO.SubTitle,
            IsApproved = true,
        };

        _dbContext.Posts.Add(post);
        _dbContext.SaveChanges();
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }
}
