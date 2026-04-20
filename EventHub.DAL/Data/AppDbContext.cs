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
                entity.Property(u => u.Password).IsRequired();

                entity.Property(u => u.ApplyAs).HasConversion<string>();
                entity.Property(u => u.Status).HasConversion<string>();
            });

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
                entity.Property(e => e.Price).HasPrecision(18, 2);

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

                entity.HasIndex(r => new { r.EventId, r.UserId }).IsUnique();

                entity.HasOne(r => r.Event)
                      .WithMany(e => e.Reviews)
                      .HasForeignKey(r => r.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Favorite ───────────────────────────────────────────────
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.HasIndex(f => new { f.UserId, f.EventId }).IsUnique();

                entity.HasOne(f => f.User)
                      .WithMany(u => u.Favorites)
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Event)
                      .WithMany(e => e.Favorites)
                      .HasForeignKey(f => f.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── EventAttachment ─────────────────────────────────────────────
            modelBuilder.Entity<EventAttachment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.FilePath).IsRequired().HasMaxLength(255);

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

                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Event)
                      .WithMany(e => e.Notifications)
                      .HasForeignKey(n => n.EventId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ─── Seed Data (FIXED - NO DYNAMIC VALUES) ───────────────────────

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "admin-user-1",
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@eventhub.com",
                    Password = "123456",
                    ApplyAs = UserRole.Admin,
                    Status = AccountStatus.Approved,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "1", Name = "Music", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "2", Name = "Technology", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "3", Name = "Sports", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "4", Name = "Arts", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "5", Name = "Business", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "6", Name = "Education", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "7", Name = "Food & Drink", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = "8", Name = "Health", CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}