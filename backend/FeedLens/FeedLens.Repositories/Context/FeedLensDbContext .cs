using FeedLens.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeedLens.Repositories.Context
{
    public class FeedLensDbContext : DbContext
    {
        public FeedLensDbContext(DbContextOptions<FeedLensDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(150);
                e.Property(u => u.PasswordHash).IsRequired();
            });

            // Video
            modelBuilder.Entity<Video>(e =>
            {
                e.HasKey(v => v.Id);
                e.Property(v => v.Title).IsRequired().HasMaxLength(200);
                e.Property(v => v.S3Key).IsRequired();
                e.HasOne(v => v.User)
                 .WithMany(u => u.Videos)
                 .HasForeignKey(v => v.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Like — unique constraint per user+video
            modelBuilder.Entity<Like>(e =>
            {
                e.HasKey(l => l.Id);
                e.HasIndex(l => new { l.UserId, l.VideoId }).IsUnique();
                e.HasOne(l => l.User)
                 .WithMany(u => u.Likes)
                 .HasForeignKey(l => l.UserId)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(l => l.Video)
                 .WithMany(v => v.Likes)
                 .HasForeignKey(l => l.VideoId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
