using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.DTOs;
using Tabloid.Models;

namespace Tabloid.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminActionController : ControllerBase
    {
        private readonly TabloidDbContext _dbContext;

        public AdminActionController(TabloidDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //GET: api/AdminAction/count?userId=123&actionType=demote
        [HttpGet("count")]
        public IActionResult GetAdminActionCount(
            [FromQuery] int userId,
            [FromQuery] string actionType
        )
        {
            // Parse the action type (case-insensitive)
            if (!Enum.TryParse<ActionType>(actionType, true, out var parsedAction))
            {
                return BadRequest("Invalid action type. Use 'Demote' or 'Deactivate'.");
            }

            // Count the number of AdminAction records for the specified user and action type
            var actionCount = _dbContext.AdminActions.Count(a =>
                a.UserProfileId == userId && a.Action == parsedAction
            );

            return Ok(actionCount);
        }

        // POST: api/AdminAction/execute?userId=123&actionType=demote
        [HttpPost("execute")]
        public IActionResult ExecuteAdminAction(
            [FromQuery] int userId,
            [FromQuery] string actionType
        )
        {
            // Parse the action type (case-insensitive)
            if (!Enum.TryParse<ActionType>(actionType, true, out var parsedAction))
            {
                return BadRequest("Invalid action type. Use 'Demote' or 'Deactivate'.");
            }

            // Retrieve the admin's profile from claims
            var adminIdentity = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var adminProfile = _dbContext.UserProfiles.FirstOrDefault(up =>
                up.IdentityUserId == adminIdentity
            );
            if (adminProfile == null)
            {
                return Unauthorized("Admin profile not found.");
            }

            // Prevent self-demotion
            if (adminProfile.Id == userId && parsedAction == ActionType.Demote)
            {
                return BadRequest("You cannot demote yourself.");
            }

            // Check if the admin has already executed the same action for the same user
            var existingAction = _dbContext.AdminActions.FirstOrDefault(a =>
                a.AdminId == adminProfile.Id
                && a.UserProfileId == userId
                && a.Action == parsedAction
            );
            if (existingAction != null)
            {
                return BadRequest("You have already executed this action for the specified user.");
            }

            // Post a new AdminAction record
            var newAction = new AdminAction
            {
                AdminId = adminProfile.Id,
                UserProfileId = userId,
                Action = parsedAction,
            };

            _dbContext.AdminActions.Add(newAction);
            _dbContext.SaveChanges();

            // Check if there are at least 2 entries for this target user and action
            var actionCount = _dbContext.AdminActions.Count(a =>
                a.UserProfileId == userId && a.Action == parsedAction
            );
            bool isApproved = actionCount >= 2;
            string message = isApproved
                ? "Action approved and execution initiated."
                : "Action recorded. Awaiting additional approval.";

            // If approved, execute the corresponding action
            if (isApproved)
            {
                if (parsedAction == ActionType.Demote)
                {
                    // Demotion Logic
                    var adminRole = _dbContext.Roles.SingleOrDefault(r =>
                        r.Name.ToLower() == "admin"
                    );
                    if (adminRole == null)
                    {
                        return NotFound("Admin role not found.");
                    }

                    var adminCount = _dbContext.UserRoles.Count(ur => ur.RoleId == adminRole.Id);
                    if (adminCount <= 1)
                    {
                        return BadRequest("Cannot demote the last admin.");
                    }

                    var targetProfile = _dbContext.UserProfiles.SingleOrDefault(up =>
                        up.Id == userId
                    );
                    if (targetProfile == null)
                    {
                        return NotFound("Target user not found.");
                    }

                    var userRole = _dbContext.UserRoles.SingleOrDefault(ur =>
                        ur.RoleId == adminRole.Id && ur.UserId == targetProfile.IdentityUserId
                    );

                    if (userRole == null)
                    {
                        return NotFound("User does not have the Admin role.");
                    }

                    _dbContext.UserRoles.Remove(userRole);
                }
                else if (parsedAction == ActionType.Deactivate)
                {
                    // Deactivation Logic
                    var userProfile = _dbContext.UserProfiles.SingleOrDefault(up =>
                        up.Id == userId
                    );
                    if (userProfile == null)
                    {
                        return NotFound("User not found.");
                    }

                    var adminRole = _dbContext.Roles.SingleOrDefault(r =>
                        r.Name.ToLower() == "admin"
                    );
                    bool isUserAdmin = _dbContext.UserRoles.Any(ur =>
                        ur.UserId == userProfile.IdentityUserId && ur.RoleId == adminRole.Id
                    );

                    if (isUserAdmin)
                    {
                        var activeAdminCount = _dbContext.UserProfiles.Count(up =>
                            up.IsActive
                            && _dbContext.UserRoles.Any(ur =>
                                ur.UserId == up.IdentityUserId && ur.RoleId == adminRole.Id
                            )
                        );

                        if (activeAdminCount <= 1 && userProfile.IsActive)
                        {
                            return BadRequest("Cannot deactivate the last active admin.");
                        }
                    }

                    userProfile.IsActive = false;
                    _dbContext.UserProfiles.Update(userProfile);
                }

                // Remove all AdminAction records for this target and action type
                var actionsToRemove = _dbContext.AdminActions.Where(a =>
                    a.UserProfileId == userId && a.Action == parsedAction
                );
                _dbContext.AdminActions.RemoveRange(actionsToRemove);
                _dbContext.SaveChanges();
            }

            var response = new AdminActionResponseDTO
            {
                IsApproved = isApproved,
                Message = message,
            };
            return Ok(response);
        }
    }
}
