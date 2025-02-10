using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tabloid.Models;

public class Subscription
{
    public int Id { get; set; }

    [Required]
    public int SubscriberId { get; set; }

    [ForeignKey("SubscriberId")]
    public UserProfile Subscriber { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfile Author { get; set; }
}
