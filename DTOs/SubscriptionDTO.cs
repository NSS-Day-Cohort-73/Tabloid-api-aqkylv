namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SubscriptionDTO
{
    public int Id { get; set; }

    public int SubscriberId { get; set; }

    [ForeignKey("SubscriberId")]
    public UserProfileDTO Subscriber { get; set; }

    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfileDTO Author { get; set; }

    public DateTime SubscriptionStartDate { get; set; }
}
