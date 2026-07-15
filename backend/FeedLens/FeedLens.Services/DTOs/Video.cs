using System.ComponentModel.DataAnnotations;

namespace FeedLens.Services.DTOs
{
    public class VideoUploadDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        public string? Tags { get; set; }

        [Required]
        public string S3Key { get; set; } = string.Empty;

        public string? ThumbnailS3Key { get; set; }
    }
    public class VideoResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIcon { get; set; }
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
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
    }
}
