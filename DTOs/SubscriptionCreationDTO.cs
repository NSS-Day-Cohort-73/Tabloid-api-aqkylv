using Microsoft.AspNetCore.Identity;

namespace Tabloid.DTOs;

public class SubscriptionCreationDTO
{
    public int Id { get; set; }

    public int SubscriberId { get; set; }

    public int AuthorId { get; set; }
}
