using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace FeedLens.Repositories.Repositories
{
    public class WatchHistoryRepository : IWatchHistoryRepository
    {
        private readonly FeedLensDbContext _context;

        public WatchHistoryRepository(FeedLensDbContext context) => _context = context;

        public async Task RecordAsync(WatchHistory history)
        {
            _context.WatchHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WatchHistory>> GetByUserIdAsync(int userId) =>
            await _context.WatchHistories
                .Include(w => w.Video)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.WatchedAt)
                .ToListAsync();
    }
}
