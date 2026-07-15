using Amazon.S3;
using Amazon.S3.Model;
using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Services.DTOs;
using FeedLens.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FeedLens.Services.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IAmazonS3 _s3;
        private readonly IConfiguration _config;

        public VideoService(IVideoRepository videoRepo, ILikeRepository likeRepo, IAmazonS3 s3, IConfiguration config)
        {
            _videoRepo = videoRepo;
            _likeRepo = likeRepo;
            _s3 = s3;
            _config = config;
        }

        public async Task<ApiResponse<VideoResponseDto>> UploadAsync(int userId, VideoUploadDto request)
        {
            try
            {
                var video = new Domain.Entities.Video
                {
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Tags = request.Tags,
                    S3Key = request.S3Key,
                    ThumbnailS3Key = request.ThumbnailS3Key,
                    UserId = userId
                };

                var created = await _videoRepo.CreateAsync(video);
                var dto = await MapToDtoAsync(created, userId);
                return ApiResponse<VideoResponseDto>.Success(dto, "Video uploaded successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<VideoResponseDto>.Failure($"Upload failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<VideoResponseDto>> GetByIdAsync(int videoId, int? currentUserId)
        {
            try
            {
                var video = await _videoRepo.GetByIdAsync(videoId);
                if (video == null)
                    return ApiResponse<VideoResponseDto>.Failure("Video not found");

                var dto = await MapToDtoAsync(video, currentUserId);
                return ApiResponse<VideoResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<VideoResponseDto>.Failure($"Failed to get video: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<VideoResponseDto>>> GetAllAsync(int? currentUserId)
        {
            try
            {
                var videos = await _videoRepo.GetAllAsync();
                var dtos = new List<VideoResponseDto>();
                foreach (var v in videos)
                    dtos.Add(await MapToDtoAsync(v, currentUserId));
                return ApiResponse<IEnumerable<VideoResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<VideoResponseDto>>.Failure($"Failed to load videos: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<VideoResponseDto>>> SearchAsync(string query, int? currentUserId)
        {
            try
            {
                var videos = await _videoRepo.SearchAsync(query);
                var dtos = new List<VideoResponseDto>();
                foreach (var v in videos)
                    dtos.Add(await MapToDtoAsync(v, currentUserId));
                return ApiResponse<IEnumerable<VideoResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<VideoResponseDto>>.Failure($"Search failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<VideoResponseDto>>> GetMyVideosAsync(int userId)
        {
            try
            {
                var videos = await _videoRepo.GetByUserIdAsync(userId);
                var dtos = new List<VideoResponseDto>();
                foreach (var v in videos)
                    dtos.Add(await MapToDtoAsync(v, userId));
                return ApiResponse<IEnumerable<VideoResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<VideoResponseDto>>.Failure($"Failed to load your videos: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UploadUrlResponseDto>> GetUploadUrlAsync(string fileName, string contentType)
        {
            try
            {
                var key = $"videos/{Guid.NewGuid()}/{fileName}";
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _config["AWS:BucketName"],
                    Key = key,
                    Verb = HttpVerb.PUT,
                    ContentType = contentType,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                };

                var url = await _s3.GetPreSignedURLAsync(request);
                return ApiResponse<UploadUrlResponseDto>.Success(new UploadUrlResponseDto
                {
                    UploadUrl = url,
                    S3Key = key
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<UploadUrlResponseDto>.Failure($"Failed to generate upload URL: {ex.Message}");
            }
        }
        private async Task<VideoResponseDto> MapToDtoAsync(Domain.Entities.Video video, int? currentUserId)
        {
            var likeCount = await _likeRepo.GetCountAsync(video.Id);
            var isLiked = currentUserId.HasValue && await _likeRepo.GetAsync(currentUserId.Value, video.Id) != null;
            var videoUrl = GeneratePresignedUrl(video.S3Key);
            var thumbnailUrl = video.ThumbnailS3Key != null ? GeneratePresignedUrl(video.ThumbnailS3Key) : null;

            return new VideoResponseDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                Category = video.Category,
                Tags = video.Tags,
                VideoUrl = videoUrl,
                ThumbnailUrl = thumbnailUrl,
                ViewCount = video.ViewCount,
                LikeCount = likeCount,
                IsLikedByCurrentUser = isLiked,
                UserId = video.UserId,
                UploaderName = video.User?.FullName ?? string.Empty,
                CreatedAt = video.CreatedAt
            };
        }

        private string GeneratePresignedUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _config["Aws:BucketName"],
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(2)
            };
            return _s3.GetPreSignedURL(request);
        }
    }
}
