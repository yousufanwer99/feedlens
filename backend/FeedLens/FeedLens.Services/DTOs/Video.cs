namespace FeedLens.Services.DTOs
{
    public class VideoUploadDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public string S3Key { get; set; } = string.Empty;
        public string? ThumbnailS3Key { get; set; }
    }
    public class VideoResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public int UserId { get; set; }
        public string UploaderName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    public class VideoSearchDto
    {
        public string Query { get; set; } = string.Empty;
    }
    public class UploadUrlResponseDto
    {
        public string UploadUrl { get; set; } = string.Empty;
        public string S3Key { get; set; } = string.Empty;
    }
}
