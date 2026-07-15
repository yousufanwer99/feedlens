using FeedLens.Helpers;
using FeedLens.Services.DTOs;

namespace FeedLens.Services.Interfaces
{
    public interface IVideoService
    {
        Task<ApiResponse<VideoResponseDto>> UploadAsync(int userId, VideoUploadDto request);
        Task<ApiResponse<VideoResponseDto>> GetByIdAsync(int videoId, int? currentUserId);
        Task<ApiResponse<IEnumerable<VideoResponseDto>>> GetAllAsync(int? currentUserId);
        Task<ApiResponse<IEnumerable<VideoResponseDto>>> SearchAsync(string query, int? currentUserId);
        Task<ApiResponse<IEnumerable<VideoResponseDto>>> GetMyVideosAsync(int userId);
        Task<ApiResponse<UploadUrlResponseDto>> GetUploadUrlAsync(string fileName, string contentType);
        Task<ApiResponse<bool>> DeleteAsync(int videoId, int userId);
    }
}
