namespace Tabloid.Models;

public class AdminApproval
{
    public int Id { get; set; }

    // The admin who approved the action.
    public int ApproverAdminId { get; set; }
    public UserProfile ApproverAdmin { get; set; }

    // Reference back to the AdminAction.
    public int AdminActionId { get; set; }
    public AdminAction AdminAction { get; set; }
}
