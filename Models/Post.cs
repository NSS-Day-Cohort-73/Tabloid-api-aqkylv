using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tabloid.Models;

public class Post
{
    public int Id { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfile Author { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string SubTitle { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    [Required]
    public DateTime PublishingDate { get; set; }

    public string HeaderImage { get; set; } = "";

    [Required]
    public string Content { get; set; }

    [NotMapped]
    public int ReadTime => (int)Math.Ceiling((double)Content.Length / 500);

    public List<Comment> Comments = new();

    public List<PostReaction> PostReactions = new();
    public List<Tag> Tags = new();
}
