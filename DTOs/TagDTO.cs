namespace Tabloid.DTOs;

public class TagDTO
{
    public int Id { get; set; }

    public string Name { get; set; }
    public List<PostDTO> Posts { get; set; } = new();
}
