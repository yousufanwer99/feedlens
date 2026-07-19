namespace FeedLens.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Algorithm preferences (feeds ML later)
        public string? PreferredCategories { get; set; }  // JSON array e.g. ["tech","finance"]
        public string? AvoidCategories { get; set; }       // JSON array

        public ICollection<Video> Videos { get; set; } = new List<Video>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public string AlgorithmMode { get; set; } = "Spectrum";
    }
}
