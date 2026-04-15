using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventHub.DAL.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IEventRepository Events { get; }
        public ITicketRepository Tickets { get; }
        public IReviewRepository Reviews { get; }
        public IFavoriteRepository Favorites { get; }
        public INotificationRepository Notifications { get; }
        public IEventAttachmentRepository EventAttachments { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context       = context;
            Users          = new UserRepository(context);
            Events         = new EventRepository(context);
            Tickets        = new TicketRepository(context);
            Reviews        = new ReviewRepository(context);
            Favorites     = new FavoriteRepository(context);
            Notifications  = new NotificationRepository(context);
            EventAttachments = new EventAttachmentRepository(context);
            Categories     = new CategoryRepository(context);
        }

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync() =>
            _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
