namespace FeedLens.Domain.Entities
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string S3Key { get; set; } = string.Empty;
        public string? ThumbnailS3Key { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Tags { get; set; }   // JSON array e.g. ["dotnet","csharp"]
        public int ViewCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
