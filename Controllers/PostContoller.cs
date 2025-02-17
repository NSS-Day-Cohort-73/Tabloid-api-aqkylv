using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Security.Claims;
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
            .Include(p => p.PostReactions)
            .ThenInclude(pr => pr.Reaction)
            .Include(p => p.Tags)
            .Include(p => p.Comments)
            .Where(p => p.IsApproved == true && p.PublishingDate < DateTime.Now)
            .OrderByDescending(p => p.PublishingDate)
            .Select(p => new PostDTO
            {
                Id = p.Id,
                Title = p.Title,
                SubTitle = p.SubTitle,
                PublishingDate = p.PublishingDate,
                IsApproved = p.IsApproved,
                ReadTime = p.ReadTime,
                Author = new UserProfileDTO { Id = p.Author.Id, FullName = p.Author.FullName },
                Category = new CategoryDTO { Id = p.Category.Id, Name = p.Category.Name },
                Content = p.Content,
                HeaderImage = p.HeaderImage,
                Comments = p
                    .Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        Subject = c.Subject,
                        Content = c.Content,
                        CreationDate = c.CreationDate,
                        Author = new UserProfileDTO
                        {
                            Id = c.Author.Id,
                            FullName = c.Author.FullName,
                        },
                    })
                    .ToList(),
                Reactions = p
                    .PostReactions.Select(pr => new ReactionDTO
                    {
                        Id = pr.Reaction.Id,
                        Name = pr.Reaction.Name,
                        Icon = pr.Reaction.Icon,
                    })
                    .ToList(),
                Tags = p.Tags.Select(t => new TagDTO { Id = t.Id, Name = t.Name }).ToList(),
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

    [HttpGet("my-posts")]
    //[Authorize]
    public IActionResult GetMyPosts()
    {
        string identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var posts = _dbContext
            .Posts.Where(p => p.Author.IdentityUserId == identityUserId)
            .Include(p => p.Author)
            .Include(p => p.Category)
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
