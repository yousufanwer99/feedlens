using Amazon.S3;
using Amazon.S3.Model;
using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Services.DTOs;
using FeedLens.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FeedLens.Services.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IWatchHistoryRepository _watchRepo;
        private readonly IUserRepository _userRepo;
        private readonly IAmazonS3 _s3;
        private readonly IConfiguration _config;

        public VideoService(
            IVideoRepository videoRepo,
            ILikeRepository likeRepo,
            IWatchHistoryRepository watchRepo,
            IUserRepository userRepo,
            IAmazonS3 s3,
            IConfiguration config)
        {
            _videoRepo = videoRepo;
            _likeRepo = likeRepo;
            _watchRepo = watchRepo;
            _userRepo = userRepo;
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
                    CategoryId = request.CategoryId,
                    Tags = request.Tags,
                    S3Key = request.S3Key,
                    ThumbnailS3Key = request.ThumbnailS3Key,
                    UserId = userId
                };

                var created = await _videoRepo.CreateAsync(video);
                var full = await _videoRepo.GetByIdAsync(created.Id);
                var dto = await MapToDtoAsync(full!, userId);
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

                await _videoRepo.IncrementViewCountAsync(videoId);

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

        public async Task<ApiResponse<bool>> DeleteAsync(int videoId, int userId)
        {
            try
            {
                var video = await _videoRepo.GetByIdAsync(videoId);
                if (video == null)
                    return ApiResponse<bool>.Failure("Video not found");
                if (video.UserId != userId)
                    return ApiResponse<bool>.Failure("Unauthorized");

                await _videoRepo.DeleteAsync(videoId);
                return ApiResponse<bool>.Success(true, "Video deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Delete failed: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> UpdateAlgorithmModeAsync(int userId, string mode)
        {
            try
            {
                var validModes = new[] { "Focal", "Prism", "Spectrum", "Flare", "Drift" };
                if (!validModes.Contains(mode))
                    return ApiResponse<bool>.Failure("Invalid algorithm mode");

                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<bool>.Failure("User not found");

                user.AlgorithmMode = mode;
                await _userRepo.UpdateAsync(user);
                return ApiResponse<bool>.Success(true, $"Mode switched to {mode}");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Failed to update mode: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<VideoResponseDto>>> GetFeedAsync(int? userId, string mode)
        {
            try
            {
                IEnumerable<Domain.Entities.Video> videos;
                var preferredCategories = new List<string>();

                if (userId.HasValue)
                {
                    var user = await _userRepo.GetByIdAsync(userId.Value);
                    if (user?.PreferredCategories != null)
                    {
                        try { preferredCategories = System.Text.Json.JsonSerializer.Deserialize<List<string>>(user.PreferredCategories) ?? new(); }
                        catch { preferredCategories = new(); }
                    }
                }

                switch (mode)
                {
                    case "Flare":
                        videos = await _videoRepo.GetFlareAsync(preferredCategories);
                        break;

                    case "Drift":
                        var watchedCategories = userId.HasValue
                            ? (await _watchRepo.GetWatchedCategoryNamesAsync(userId.Value)).ToList()
                            : new List<string>();
                        videos = await _videoRepo.GetDriftAsync(userId ?? 0, watchedCategories);
                        break;

                    default:
                        // Focal, Prism, Spectrum — fall back to all videos for now
                        videos = await _videoRepo.GetAllAsync();
                        break;
                }

                var dtos = new List<VideoResponseDto>();
                foreach (var v in videos)
                    dtos.Add(await MapToDtoAsync(v, userId));

                return ApiResponse<IEnumerable<VideoResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<VideoResponseDto>>.Failure($"Failed to load feed: {ex.Message}");
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
                CategoryId = video.CategoryId,
                CategoryName = video.Category?.Name ?? string.Empty,
                CategoryIcon = video.Category?.Icon,
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
