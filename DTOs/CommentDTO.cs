namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations.Schema;

public class CommentDTO
{
    public int Id { get; set; }

    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfileDTO Author { get; set; }

    public int PostId { get; set; }
    public PostDTO Post { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }
}
