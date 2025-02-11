namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations.Schema;

public class PostDTO
{
    public int Id { get; set; }

    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfileDTO Author { get; set; }

    public string Title { get; set; }

    public string SubTitle { get; set; }

    public int CategoryId { get; set; }

    public CategoryDTO Category { get; set; }

    public DateTime PublishingDate { get; set; }

    public string HeaderImage { get; set; } = "";

    public string Content { get; set; }

    public int ReadTime { get; set; }

    public List<CommentDTO> Comments = new();
    public List<ReactionDTO> Reactions = new();
    public List<TagDTO> Tags = new();

    public bool IsApproved { get; set; } = false;
}
