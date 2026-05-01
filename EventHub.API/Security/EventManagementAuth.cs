using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventHub.Domain.Enums;

namespace EventHub.API.Security
{
    public static class EventManagementAuth
    {
        public static string? GetUserId(ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        public static bool IsAdmin(ClaimsPrincipal user) =>
            user.IsInRole(nameof(UserRole.Admin));

        /// <summary>Admin or the event's owning approved organizer (JWT role EventOrganizer).</summary>
        public static bool CanMutateEvent(ClaimsPrincipal user, string eventOrganizerId)
        {
            if (IsAdmin(user))
                return true;
            if (!user.IsInRole(nameof(UserRole.EventOrganizer)))
                return false;
            var userId = GetUserId(user);
            return !string.IsNullOrEmpty(userId) && userId == eventOrganizerId;
        }
    }
}
