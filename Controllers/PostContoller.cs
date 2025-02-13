using System.Linq.Expressions;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public IActionResult GetById(int id)
    {
        var postById = _dbContext
            .Posts.Include(p => p.Author)
            .ThenInclude(a => a.IdentityUser)
            .SingleOrDefault(p => p.Id == id);

        if (postById == null)
        {
            return NotFound();
        }
        var thisPost = new GetPostByIdDTO
        {
            Id = postById.Id,
            Title = postById.Title,
            PublishingDate = postById.PublishingDate,
            Content = postById.Content,
            HeaderImage = postById.HeaderImage,
            AuthorId = postById.AuthorId,
            Author = new UserProfileForPostByIdDTO
            {
                Id = postById.Author.Id,
                IdentityUser = new IdentityUserDTO
                {
                    Id = postById.Author.IdentityUser.Id,
                    UserName = postById.Author.IdentityUser.UserName,
                },
            },
        };

        return Ok(thisPost);
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
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpDelete("{id}")]
    //[Authorize]
    public IActionResult Delete(int id)
    {
        Post post = _dbContext.Posts.SingleOrDefault(p => p.Id == id);

        if (post == null)
        {
            return NotFound("There Post Id does not exist");
        }

        _dbContext.Posts.Remove(post);
        _dbContext.SaveChanges();
        return NoContent();
    }
}
