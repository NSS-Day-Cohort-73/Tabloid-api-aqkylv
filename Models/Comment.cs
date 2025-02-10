using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tabloid.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfile Author { get; set; }

    [Required]
    public int PostId { get; set; }
    public Post Post { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime CreationDate { get; set; }
}
