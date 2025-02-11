namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations.Schema;

public class GetPostListDTO
{
    public int Id { get; set; }
    
    [ForeignKey("AuthorId")]
    public UserProfileDTO Author { get; set; }

    public string Title { get; set; }

    public CategoryDTO Category { get; set; }

    public DateTime PublishingDate { get; set; }

   
    public bool IsApproved { get; set; } = false;
}
