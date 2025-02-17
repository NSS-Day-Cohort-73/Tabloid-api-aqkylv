namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations.Schema;

public class EditPostDTO
{
    public int Id { get; set; }

    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public UserProfileForPostByIdDTO Author { get; set; }

    public string Title { get; set; }

    public string SubTitle { get; set; }

    public CategoryDTO Category { get; set; }

    public DateTime PublishingDate { get; set; }

    public string HeaderImage { get; set; } = "";

    public string Content { get; set; }


}