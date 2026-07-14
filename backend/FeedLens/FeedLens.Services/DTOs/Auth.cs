namespace FeedLens.Services.DTOs
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PreferredCategories { get; set; }
        public string? AvoidCategories { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? PreferredCategories { get; set; }
        public string? AvoidCategories { get; set; }
    }
}
