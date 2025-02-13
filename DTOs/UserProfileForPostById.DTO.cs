using Microsoft.AspNetCore.Identity;

namespace Tabloid.DTOs;

public class UserProfileForPostByIdDTO
{
    public int Id { get; set; }

    public string IdentityUserId { get; set; }

    public IdentityUserDTO IdentityUser { get; set; }
}

   