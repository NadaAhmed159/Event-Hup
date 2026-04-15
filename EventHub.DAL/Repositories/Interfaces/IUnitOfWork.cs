namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IEventRepository Events { get; }
        ITicketRepository Tickets { get; }
        IReviewRepository Reviews { get; }
        IFavoriteRepository Favorites { get; }
        INotificationRepository Notifications { get; }
        IEventAttachmentRepository EventAttachments { get; }
        ICategoryRepository Categories { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
