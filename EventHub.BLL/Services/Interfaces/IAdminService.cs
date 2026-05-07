namespace EventHub.BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task ApproveOrganizerAsync(string userId, CancellationToken cancellationToken = default);
        Task RejectOrganizerAsync(string userId, CancellationToken cancellationToken = default);
        Task ApproveEventAsync(string eventId, CancellationToken cancellationToken = default);
        Task RejectEventAsync(string eventId, CancellationToken cancellationToken = default);
    }
}
