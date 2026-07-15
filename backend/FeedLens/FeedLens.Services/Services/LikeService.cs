using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Services.Interfaces;

namespace FeedLens.Services.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepo;
        private readonly IVideoRepository _videoRepo;

        public LikeService(ILikeRepository likeRepo, IVideoRepository videoRepo)
        {
            _likeRepo = likeRepo;
            _videoRepo = videoRepo;
        }

        public async Task<ApiResponse<bool>> ToggleLikeAsync(int userId, int videoId)
        {
            try
            {
                var video = await _videoRepo.GetByIdAsync(videoId);
                if (video == null)
                    return ApiResponse<bool>.Failure("Video not found");

                var existing = await _likeRepo.GetAsync(userId, videoId);
                if (existing != null)
                {
                    await _likeRepo.DeleteAsync(userId, videoId);
                    return ApiResponse<bool>.Success(false, "Video unliked");
                }

                await _likeRepo.CreateAsync(new Like { UserId = userId, VideoId = videoId });
                return ApiResponse<bool>.Success(true, "Video liked");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Failed to toggle like: {ex.Message}");
            }
        }
    }
}
