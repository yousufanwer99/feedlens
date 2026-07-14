using FeedLens.Helpers;

namespace FeedLens.Services.Interfaces
{
    public interface ILikeService
    {
        Task<ApiResponse<bool>> ToggleLikeAsync(int userId, int videoId);
    }
}
