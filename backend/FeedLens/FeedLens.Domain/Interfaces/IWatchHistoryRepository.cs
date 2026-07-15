using FeedLens.Domain.Entities;

namespace FeedLens.Domain.Interfaces
{
    public interface IWatchHistoryRepository
    {
        Task RecordAsync(WatchHistory history);
        Task<IEnumerable<WatchHistory>> GetByUserIdAsync(int userId);
    }
}
