using FeedLens.Domain.Entities;

namespace FeedLens.Domain.Interfaces
{
    public interface ILikeRepository
    {
        Task<Like?> GetAsync(int userId, int videoId);
        Task<int> GetCountAsync(int videoId);
        Task<Like> CreateAsync(Like like);
        Task DeleteAsync(int userId, int videoId);
    }
}
