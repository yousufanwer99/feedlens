using FeedLens.Domain.Entities;
using FeedLens.Helpers;

namespace FeedLens.Domain.Interfaces
{
    public interface IVideoRepository
    {
        Task<Video?> GetByIdAsync(int id);
        Task<IEnumerable<Video>> GetAllAsync();
        Task<IEnumerable<Video>> SearchAsync(string query);
        Task<IEnumerable<Video>> GetByUserIdAsync(int userId);
        Task<Video> CreateAsync(Video video);
        Task<Video> UpdateAsync(Video video);
        Task DeleteAsync(int id);
        Task IncrementViewCountAsync(int videoId);
        Task<IEnumerable<Video>> GetFlareAsync(IEnumerable<string> preferredCategories);
        Task<IEnumerable<Video>> GetDriftAsync(int userId, IEnumerable<string> watchedCategoryNames);
    }
}
