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
[Authorize]
public IActionResult Get(
    [FromQuery] int? categoryId,
    [FromQuery] int? tagId,
    [FromQuery] bool? approved,
    [FromQuery] bool? subscribed = false
)
{
    string currentUserIdentityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var currentUserProfile = _dbContext.UserProfiles.SingleOrDefault(up =>
        up.IdentityUserId == currentUserIdentityId
    );
    
    if (currentUserProfile == null)
    {
        return Unauthorized();
    }

    bool isAdmin = User.IsInRole("Admin");
    bool targetApproved = !(approved.HasValue && !approved.Value && isAdmin);

    var query = _dbContext
        .Posts.Include(p => p.Author)
        .Include(p => p.Category)
        .Include(p => p.PostReactions)
        .ThenInclude(pr => pr.Reaction)
        .Include(p => p.PostTags)
        .ThenInclude(pt => pt.Tag)
        .Include(p => p.Comments)
        .Where(p => p.IsApproved == targetApproved)
        .Where(p => p.PublishingDate < DateTime.Now);

    if (subscribed == true)
    {
        // Get IDs of all authors the current user is subscribed to
        var subscribedAuthorIds = _dbContext.Subscriptions
            .Where(s => s.SubscriberId == currentUserProfile.Id)
            .Where(s => s.SubscriptionEndDate == null || s.SubscriptionEndDate > DateTime.Now)
            .Select(s => s.AuthorId)
            .ToList();

        // Filter posts to only include those from subscribed authors
        query = query.Where(p => subscribedAuthorIds.Contains(p.AuthorId));
    }

    if (categoryId.HasValue)
    {
        query = query.Where(p => p.CategoryId == categoryId.Value);
    }
    if (tagId.HasValue)
    {
        query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Id == tagId.Value));
    }

    var posts = query
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
            Tags = p
                .PostTags.Select(pt => new TagDTO { Id = pt.Tag.Id, Name = pt.Tag.Name })
                .ToList(),
        })
        .ToList();

    return Ok(posts);
}
    [HttpGet("{id}")]
    [Authorize]
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
            SubTitle = postById.SubTitle,
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
            ReadTime = postById.ReadTime,
        };

        return Ok(thisPost);
    }

    [HttpGet("createpost/{id}")]
    [Authorize]
    public IActionResult GetByIdToEdit(int id)
    {
        var postById = _dbContext
            .Posts.Include(p => p.Author)
            .ThenInclude(a => a.IdentityUser)
            .Include(p => p.Category)
            .SingleOrDefault(p => p.Id == id);

        if (postById == null)
        {
            return NotFound();
        }
        var thisPost = new EditPostDTO
        {
            Id = postById.Id,
            Title = postById.Title,
            SubTitle = postById.SubTitle,
            PublishingDate = postById.PublishingDate,
            Category = new CategoryDTO { Id = postById.Category.Id, Name = postById.Category.Name },
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
    [Authorize]
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
    [Authorize]
    public IActionResult Post(CreatePostDTO postDTO)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.SingleOrDefault(up =>
            up.IdentityUserId == identityUserId
        );
        if (userProfile == null)
            return Unauthorized();

        var post = new Post
        {
            Title = postDTO.Title,
            Content = postDTO.Content,
            CategoryId = postDTO.CategoryId,
            AuthorId = userProfile.Id,
            PublishingDate = DateTime.Now,
            SubTitle = postDTO.SubTitle,
            HeaderImage = postDTO.HeaderImage,
            IsApproved = false,
        };

        bool isAdmin = User.IsInRole("Admin");
        if (isAdmin)
        {
            post.IsApproved = true;
        }

        _dbContext.Posts.Add(post);
        _dbContext.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Put(int id, CreatePostDTO postDTO)
    {
        var postToUpdate = _dbContext.Posts.SingleOrDefault(p => p.Id == id);
        if (postToUpdate == null)
            return NotFound();

        postToUpdate.Title = postDTO.Title;
        postToUpdate.SubTitle = postDTO.SubTitle;
        postToUpdate.CategoryId = postDTO.CategoryId;
        postToUpdate.Content = postDTO.Content;
        postToUpdate.HeaderImage = postDTO.HeaderImage;

        _dbContext.SaveChanges();
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize]
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

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public IActionResult ApprovePost(int id)
    {
        var post = _dbContext.Posts.SingleOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound("Post not found.");
        }

        post.IsApproved = true;
        _dbContext.SaveChanges();
        return Ok("Post approved.");
    }

    [HttpPost("{id}/unapprove")]
    [Authorize(Roles = "Admin")]
    public IActionResult UnapprovePost(int id)
    {
        var post = _dbContext.Posts.SingleOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound("Post not found.");
        }

        post.IsApproved = false;
        _dbContext.SaveChanges();
        return Ok("Post unapproved.");
    }

}
