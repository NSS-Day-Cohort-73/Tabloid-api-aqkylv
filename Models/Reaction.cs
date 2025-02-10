using System.ComponentModel.DataAnnotations;

namespace Tabloid.Models;

public class Reaction
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Icon { get; set; }
}
