using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EventAttachment> EventAttachments { get; set; }

        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── User ───────────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).HasConversion<string>();
                entity.Property(u => u.AccountStatus).HasConversion<string>();

                // Admin accounts are auto-approved
                entity.HasQueryFilter(u =>
                    u.Role != UserRole.EventOrganizer ||
                    u.AccountStatus == AccountStatus.Approved);
            });

            modelBuilder.Entity<User>().HasData(
        new User
        {
            Id = "",
            Name = "Admin",
            Email = "admin@eventhub.com",
            Password = "123456", // هنحسنها تحت 👇
            Role = "admin",
            Status = "accepted",
            CreatedAt = DateTime.Now
        }
    );

            // ─── Category ───────────────────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(c => c.Name).IsUnique();
            });

            // ─── Event ──────────────────────────────────────────────────────
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Venue).IsRequired().HasMaxLength(300);
                entity.Property(e => e.TicketPrice).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.Organizer)
                      .WithMany(u => u.OrganizedEvents)
                      .HasForeignKey(e => e.OrganizerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Events)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Ticket ─────────────────────────────────────────────────────
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.QrCode).IsRequired();
                entity.HasIndex(t => t.QrCode).IsUnique();
                entity.Property(t => t.UniqueCode).IsRequired().HasMaxLength(50);
                entity.HasIndex(t => t.UniqueCode).IsUnique();
                entity.Property(t => t.PricePaid).HasPrecision(18, 2);
                entity.Property(t => t.Status).HasConversion<string>();

                entity.HasOne(t => t.Event)
                      .WithMany(e => e.Tickets)
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Participant)
                      .WithMany(u => u.Tickets)
                      .HasForeignKey(t => t.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Review ─────────────────────────────────────────────────────
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Rating).IsRequired();

                // One review per participant per event
                entity.HasIndex(r => new { r.EventId, r.ParticipantId }).IsUnique();

                entity.HasOne(r => r.Event)
                      .WithMany(e => e.Reviews)
                      .HasForeignKey(r => r.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Participant)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── WatchlistItem ───────────────────────────────────────────────
            modelBuilder.Entity<WatchlistItem>(entity =>
            {
                entity.HasKey(w => w.Id);

                // One save per user per event
                entity.HasIndex(w => new { w.UserId, w.EventId }).IsUnique();

                entity.HasOne(w => w.User)
                      .WithMany(u => u.WatchlistItems)
                      .HasForeignKey(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(w => w.Event)
                      .WithMany(e => e.WatchlistItems)
                      .HasForeignKey(w => w.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── EventAttachment ─────────────────────────────────────────────
            modelBuilder.Entity<EventAttachment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.FileName).IsRequired().HasMaxLength(255);
                entity.Property(a => a.FileUrl).IsRequired();
                entity.Property(a => a.FileType).IsRequired().HasMaxLength(50);

                entity.HasOne(a => a.Event)
                      .WithMany(e => e.Attachments)
                      .HasForeignKey(a => a.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Notification ────────────────────────────────────────────────
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.Message).IsRequired();
                entity.Property(n => n.Type).HasConversion<string>();

                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Event)
                      .WithMany(e => e.Notifications)
                      .HasForeignKey(n => n.EventId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ─── Seed Data ───────────────────────────────────────────────────
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Music",        CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Technology",   CreatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Sports",       CreatedAt = DateTime.UtcNow },
                new Category { Id = 4, Name = "Arts",         CreatedAt = DateTime.UtcNow },
                new Category { Id = 5, Name = "Business",     CreatedAt = DateTime.UtcNow },
                new Category { Id = 6, Name = "Education",    CreatedAt = DateTime.UtcNow },
                new Category { Id = 7, Name = "Food & Drink", CreatedAt = DateTime.UtcNow },
                new Category { Id = 8, Name = "Health",       CreatedAt = DateTime.UtcNow }
            );
        }
    }
}
