namespace FeedLens.Domain.Entities
{
    public class WatchHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        public int WatchedSeconds { get; set; }
        public int TotalSeconds { get; set; }
        public double WatchPercentage { get; set; }
        public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
    }
}
