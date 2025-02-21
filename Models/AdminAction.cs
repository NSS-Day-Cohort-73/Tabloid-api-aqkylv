namespace Tabloid.Models;

public class AdminAction
{
    public int Id { get; set; }

    // The admin who initiated or approved this action.
    public int AdminId { get; set; }
    public UserProfile Admin { get; set; }

    // The target user to be acted upon.
    public int UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }

    // The type of action: Demote or Deactivate.
    public ActionType Action { get; set; }
}
