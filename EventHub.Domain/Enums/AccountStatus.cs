namespace EventHub.Domain.Enums
{
    /// <summary>
    /// Account moderation state. Participants and admins are always <see cref="Approved"/>.
    /// New event-organizer registrations start as <see cref="Pending"/> until an admin approves.
    /// </summary>
    public enum AccountStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
