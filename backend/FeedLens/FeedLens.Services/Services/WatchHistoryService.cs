using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Services.DTOs;
using FeedLens.Services.Interfaces;

namespace FeedLens.Services.Services
{
    public class WatchHistoryService : IWatchHistoryService
    {
        private readonly IWatchHistoryRepository _watchRepo;
        private readonly IVideoRepository _videoRepo;

        public WatchHistoryService(IWatchHistoryRepository watchRepo, IVideoRepository videoRepo)
        {
            _watchRepo = watchRepo;
            _videoRepo = videoRepo;
        }

        public async Task<ApiResponse<bool>> RecordWatchAsync(int userId, int videoId, WatchRequestDto request)
        {
            try
            {
                var video = await _videoRepo.GetByIdAsync(videoId);
                if (video == null)
                    return ApiResponse<bool>.Failure("Video not found");

                if (request.TotalSeconds <= 0)
                    return ApiResponse<bool>.Failure("Invalid video duration");

                var percentage = (double)request.WatchedSeconds / request.TotalSeconds * 100;

                // Only record if watched more than 30%
                if (percentage < 30)
                    return ApiResponse<bool>.Success(false, "Watch threshold not reached");

                var history = new WatchHistory
                {
                    UserId = userId,
                    VideoId = videoId,
                    WatchedSeconds = request.WatchedSeconds,
                    TotalSeconds = request.TotalSeconds,
                    WatchPercentage = Math.Round(percentage, 2),
                    WatchedAt = DateTime.UtcNow
                };

                await _watchRepo.RecordAsync(history);
                return ApiResponse<bool>.Success(true, "Watch recorded");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Failed to record watch: {ex.Message}");
            }
        }
    }
}
