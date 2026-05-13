using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EventHub.API.Hubs
{
    /// <summary>Live ticket counts per event (<c>JoinEvent</c>) and push notifications to signed-in users (<c>OnConnectedAsync</c> adds each user to their own group).</summary>
    [AllowAnonymous]
    public sealed class EventAvailabilityHub : Hub
    {
        public static string GroupName(string eventId) => $"event-{eventId}";
        public static string UserGroup(string userId) => $"user-{userId}";

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
            await base.OnConnectedAsync();
        }

        public Task JoinEvent(string eventId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, GroupName(eventId));

        public Task LeaveEvent(string eventId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(eventId));
    }
}
