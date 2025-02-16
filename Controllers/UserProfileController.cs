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
public class UserProfileController : ControllerBase
{
    private TabloidDbContext _dbContext;

    public UserProfileController(TabloidDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.UserProfiles.ToList());
    }

    [HttpGet("withroles")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetWithRoles()
    {
        return Ok(
            _dbContext
                .UserProfiles.Include(up => up.IdentityUser)
                .Select(up => new UserProfile
                {
                    Id = up.Id,
                    FirstName = up.FirstName,
                    LastName = up.LastName,
                    Email = up.IdentityUser.Email,
                    UserName = up.IdentityUser.UserName,
                    IdentityUserId = up.IdentityUserId,
                    Roles = _dbContext
                        .UserRoles.Where(ur => ur.UserId == up.IdentityUserId)
                        .Select(ur => _dbContext.Roles.SingleOrDefault(r => r.Id == ur.RoleId).Name)
                        .ToList(),
                    IsActive = up.IsActive,
                })
        );
    }

    [HttpPost("promote/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Promote(string id)
    {
        IdentityRole role = _dbContext.Roles.SingleOrDefault(r => r.Name == "Admin");
        _dbContext.UserRoles.Add(new IdentityUserRole<string> { RoleId = role.Id, UserId = id });
        _dbContext.SaveChanges();
        return NoContent();
    }

    [HttpPost("demote/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Demote(string id)
    {
        IdentityRole role = _dbContext.Roles.SingleOrDefault(r => r.Name == "Admin");

        IdentityUserRole<string> userRole = _dbContext.UserRoles.SingleOrDefault(ur =>
            ur.RoleId == role.Id && ur.UserId == id
        );

        _dbContext.UserRoles.Remove(userRole);
        _dbContext.SaveChanges();
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        UserProfile user = _dbContext
            .UserProfiles.Include(up => up.IdentityUser)
            .SingleOrDefault(up => up.Id == id);

        if (user == null)
        {
            return NotFound();
        }
        user.Email = user.IdentityUser.Email;
        user.UserName = user.IdentityUser.UserName;
        user.Roles = _dbContext
            .UserRoles.Where(ur => ur.UserId == user.IdentityUserId)
            .Select(ur => _dbContext.Roles.SingleOrDefault(r => r.Id == ur.RoleId).Name)
            .ToList();

        return Ok(user);
    }

    [HttpPost("deactivate/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeactivateAccount(int id)
    {
        // Find the user profile by ID
        var userProfile = _dbContext.UserProfiles.SingleOrDefault(up => up.Id == id);

        if (userProfile == null)
        {
            return NotFound("User not found.");
        }

        // Debug: Log current state of IsActive before update
        Console.WriteLine(
            $"Deactivating user with ID {id}. Current IsActive: {userProfile.IsActive}"
        );

        // Set IsActive to false to deactivate the account
        userProfile.IsActive = false; // Adjust to your property type, e.g., bool or string as required

        // Save changes
        _dbContext.SaveChanges();

        // Debug: Log after saving changes
        Console.WriteLine($"User with ID {id} deactivated. New IsActive: {userProfile.IsActive}");

        return NoContent(); // Success response
    }

    [HttpPost("reactivate/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult ReactivateAccount(int id)
    {
        // Find the user profile by ID
        var userProfile = _dbContext.UserProfiles.SingleOrDefault(up => up.Id == id);

        if (userProfile == null)
        {
            return NotFound("User not found.");
        }

        // Set IsActive to true to reactivate the account
        userProfile.IsActive = true; // Adjust to your property type, e.g., bool or string as required
        _dbContext.SaveChanges();

        return NoContent(); // Success response
    }

    [HttpGet("{id}/posts")]
    //[Authorize]
    public IActionResult GetUserProfilesPost(int id)
    {
        var posts = _dbContext
            .Posts.Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.IsApproved == true)
            .Where(p => p.PublishingDate < DateTime.Now)
            .Where(p => p.Id == id)
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

    [HttpPut("{id}/image")]
    [Authorize]
    public IActionResult UpdateUserProfileImage(int id, [FromBody] UserProfileDTO dto)
    {
        // 1. Find the user by ID
        var userProfile = _dbContext.UserProfiles.SingleOrDefault(up => up.Id == id);

        if (userProfile == null)
        {
            return NotFound("UserProfile not found.");
        }

        // 2. Update only the ImageLocation property
        userProfile.ImageLocation = dto.ImageLocation;

        // 3. Save changes
        _dbContext.SaveChanges();

        // 4. Return success response
        return NoContent();
    }
}
