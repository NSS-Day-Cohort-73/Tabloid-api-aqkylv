namespace Tabloid.DTOs;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class UserProfileDTO
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    [NotMapped]
    public string UserName { get; set; }

    [NotMapped]
    public string Email { get; set; }

    public DateTime CreateDateTime { get; set; }

    [DataType(DataType.Url)]
    [MaxLength(255)]
    public string ImageLocation { get; set; }

    [NotMapped]
    public List<string> Roles { get; set; }

    public List<PostDTO> Posts { get; set; } = new();

    public List<CommentDTO> Comments { get; set; } = new();

    public string IdentityUserId { get; set; }

    public IdentityUser IdentityUser { get; set; }

    public string FullName { get; set; }

    public Boolean IsActive { get; set; }
}