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
    public int ReadTime => (int)Math.Ceiling((double)GetWordCount(Content) / 265);

    private static int GetWordCount(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(
            new[] { ' ', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries
        ).Length;
    }

    public List<Comment> Comments { get; set; } = new();

    public List<PostReaction> PostReactions { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();

    public bool IsApproved { get; set; } = false;
}
