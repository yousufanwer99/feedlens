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
        public DbSet<Category> Categories { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
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

            // Category
            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
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
                e.HasOne(v => v.Category)
                 .WithMany()
                 .HasForeignKey(v => v.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Like
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

            // WatchHistory
            modelBuilder.Entity<WatchHistory>(e =>
            {
                e.HasKey(w => w.Id);
                e.HasOne(w => w.User)
                 .WithMany()
                 .HasForeignKey(w => w.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(w => w.Video)
                 .WithMany()
                 .HasForeignKey(w => w.VideoId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Technology", Icon = "💻", SortOrder = 1 },
                new Category { Id = 2, Name = "Finance", Icon = "💰", SortOrder = 2 },
                new Category { Id = 3, Name = "Education", Icon = "📚", SortOrder = 3 },
                new Category { Id = 4, Name = "Entertainment", Icon = "🎬", SortOrder = 4 },
                new Category { Id = 5, Name = "Sports", Icon = "⚽", SortOrder = 5 },
                new Category { Id = 6, Name = "Music", Icon = "🎵", SortOrder = 6 },
                new Category { Id = 7, Name = "Gaming", Icon = "🎮", SortOrder = 7 },
                new Category { Id = 8, Name = "Travel", Icon = "✈️", SortOrder = 8 },
                new Category { Id = 9, Name = "Food", Icon = "🍕", SortOrder = 9 },
                new Category { Id = 10, Name = "Health", Icon = "🏥", SortOrder = 10 },
                new Category { Id = 11, Name = "Science", Icon = "🔬", SortOrder = 11 },
                new Category { Id = 12, Name = "Business", Icon = "💼", SortOrder = 12 },
                new Category { Id = 13, Name = "News", Icon = "📰", SortOrder = 13 },
                new Category { Id = 14, Name = "Comedy", Icon = "😂", SortOrder = 14 },
                new Category { Id = 15, Name = "Fitness", Icon = "💪", SortOrder = 15 },
                new Category { Id = 16, Name = "Art & Design", Icon = "🎨", SortOrder = 16 },
                new Category { Id = 17, Name = "Photography", Icon = "📷", SortOrder = 17 },
                new Category { Id = 18, Name = "Programming", Icon = "👨‍💻", SortOrder = 18 },
                new Category { Id = 19, Name = "Lifestyle", Icon = "🌟", SortOrder = 19 },
                new Category { Id = 20, Name = "Other", Icon = "📌", SortOrder = 20 }
            );
        }

    }
}
