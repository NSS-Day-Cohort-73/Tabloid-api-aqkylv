using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly TabloidDbContext _dbContext;

    public SubscriptionController(TabloidDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    //[Authorize]
    public IActionResult GetSubsriptions()
    {
        var subscriptions = _dbContext
            .Subscriptions.Select(s => new SubscriptionDTO
            {
                Id = s.Id,
                AuthorId = s.AuthorId,
                Author = new UserProfileDTO { Id = s.Author.Id, FullName = s.Author.FullName },
                SubscriberId = s.SubscriberId,
                Subscriber = new UserProfileDTO { Id = s.Author.Id, FullName = s.Author.FullName },
            })
            .ToList();

        return Ok(subscriptions);
    }

    [HttpPost]
    //[Authorize]
    public IActionResult PostSubscription(SubscriptionCreationDTO subscription)
    {
        bool subscriptionExists = _dbContext.Subscriptions.Any(s =>
            s.SubscriberId == subscription.SubscriberId && s.AuthorId == subscription.AuthorId
        );

        if (subscriptionExists)
        {
            return BadRequest(new { error = "This subscription already exists." });
        }

        Subscription newSubscription = new Subscription
        {
            Id = subscription.Id,
            SubscriberId = subscription.SubscriberId,
            AuthorId = subscription.AuthorId,
            SubscriptionStartDate = DateTime.Now, 
        };

        _dbContext.Subscriptions.Add(newSubscription);
        _dbContext.SaveChanges();

        return CreatedAtAction(
            nameof(PostSubscription),
            new { id = newSubscription.Id },
            newSubscription
        );
    }
}
