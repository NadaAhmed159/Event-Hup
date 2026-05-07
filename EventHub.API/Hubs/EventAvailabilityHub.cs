using Microsoft.AspNetCore.SignalR;

namespace EventHub.API.Hubs
{
    public sealed class EventAvailabilityHub : Hub
    {
        public static string GroupName(string eventId) => $"event-{eventId}";

        public Task JoinEvent(string eventId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, GroupName(eventId));

        public Task LeaveEvent(string eventId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(eventId));
    }
}
