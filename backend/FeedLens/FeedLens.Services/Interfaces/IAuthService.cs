using FeedLens.Helpers;
using FeedLens.Services.DTOs;
namespace FeedLens.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId);
    Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto request);
}
