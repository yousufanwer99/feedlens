using FeedLens.Helpers;
using FeedLens.Services.DTOs;

namespace FeedLens.Services.Interfaces
{
    public interface IWatchHistoryService
    {
        Task<ApiResponse<bool>> RecordWatchAsync(int userId, int videoId, WatchRequestDto request);
    }
}
