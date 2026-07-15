using FeedLens.Helpers;
using FeedLens.Services.DTOs;

namespace FeedLens.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllAsync();
    }
}
