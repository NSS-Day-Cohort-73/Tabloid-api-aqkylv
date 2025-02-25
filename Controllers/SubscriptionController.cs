using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet("check/{authorId}/{subscriberId}")]
    public IActionResult CheckSubscription(int authorId, int subscriberId)
    {
        var subscription = _dbContext.Subscriptions.FirstOrDefault(s =>
            s.AuthorId == authorId && s.SubscriberId == subscriberId
        );

        if (subscription == null)
        {
            return Ok(new { isSubscribed = false });
        }

        return Ok(new { isSubscribed = true, subscriptionId = subscription.Id });
    }

    [HttpGet]
    public IActionResult GetMySubscribedAuthors()
    {
        var identityUserIdMe = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var me = _dbContext.UserProfiles.Single(up => up.IdentityUserId == identityUserIdMe);

        var MySubscriptions = _dbContext
            .Subscriptions.Where(s => s.Subscriber.IdentityUserId == me.IdentityUserId).Include(s => s.Author)
            .ToList();

        return Ok(MySubscriptions);
    }

    [HttpPost]
    public IActionResult PostSubscription(SubscriptionCreationDTO subscription)
    {
        if (subscription.AuthorId == subscription.SubscriberId)
        {
            return BadRequest(new { error = "Users cannot subscribe to themselves." });
        }

        bool subscriptionExists = _dbContext.Subscriptions.Any(s =>
            s.SubscriberId == subscription.SubscriberId && s.AuthorId == subscription.AuthorId
        );

        if (subscriptionExists)
        {
            return BadRequest(new { error = "This subscription already exists." });
        }

        Subscription newSubscription = new Subscription
        {
            SubscriberId = subscription.SubscriberId,
            AuthorId = subscription.AuthorId,
            SubscriptionStartDate = DateTime.Now,
        };

        _dbContext.Subscriptions.Add(newSubscription);
        _dbContext.SaveChanges();

        return CreatedAtAction(
            nameof(CheckSubscription),
            new
            {
                authorId = newSubscription.AuthorId,
                subscriberId = newSubscription.SubscriberId,
            },
            new { subscriptionId = newSubscription.Id }
        );
    }

    [HttpDelete("{subscriptionId}")]
    public IActionResult Unsubscribe(int subscriptionId)
    {
        Subscription subscription = _dbContext.Subscriptions.SingleOrDefault(s =>
            s.Id == subscriptionId
        );

        if (subscription == null)
        {
            return NotFound();
        }

        _dbContext.Subscriptions.Remove(subscription);
        _dbContext.SaveChanges();
        return NoContent();
    }
}
