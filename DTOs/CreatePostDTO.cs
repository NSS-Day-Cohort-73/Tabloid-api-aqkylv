namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations.Schema;

public class CreatePostDTO
{
    public int Id { get; set; }

    public int AuthorId { get; set; }


    public string Title { get; set; }

    public string SubTitle { get; set; }

    public int CategoryId { get; set; }

    public DateTime PublishingDate { get; set; }

    public string? HeaderImage { get; set; } = "";

    public string Content { get; set; }


    public bool IsApproved { get; set; } = false;
}