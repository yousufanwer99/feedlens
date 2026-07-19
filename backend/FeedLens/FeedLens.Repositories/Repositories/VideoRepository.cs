using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace FeedLens.Repositories.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly FeedLensDbContext _context;

        public VideoRepository(FeedLensDbContext context) => _context = context;

        public async Task<Video?> GetByIdAsync(int id) =>
        await _context.Videos
        .Include(v => v.User)
        .Include(v => v.Category)
        .FirstOrDefaultAsync(v => v.Id == id);

        public async Task<IEnumerable<Video>> GetAllAsync() =>
    await _context.Videos
        .Include(v => v.User)
        .Include(v => v.Category)
        .OrderByDescending(v => v.CreatedAt)
        .ToListAsync();

        public async Task<IEnumerable<Video>> SearchAsync(string query) =>
         await _context.Videos
             .Include(v => v.User)
             .Include(v => v.Category)
             .Where(v => v.Title.Contains(query) ||
                         v.Tags!.Contains(query) ||
                         v.Category.Name.Contains(query))
             .OrderByDescending(v => v.CreatedAt)
             .ToListAsync();

        public async Task<IEnumerable<Video>> GetByUserIdAsync(int userId) =>
         await _context.Videos
        .Include(v => v.Category)
        .Where(v => v.UserId == userId)
        .OrderByDescending(v => v.CreatedAt)
        .ToListAsync();

        public async Task<Video> CreateAsync(Video video)
        {
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<Video> UpdateAsync(Video video)
        {
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task DeleteAsync(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementViewCountAsync(int videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video != null)
            {
                video.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Video>> GetFlareAsync(IEnumerable<string> preferredCategories)
        {
            var since = DateTime.UtcNow.AddDays(-7);
            var query = _context.Videos
                .Include(v => v.User)
                .Include(v => v.Category)
                .Include(v => v.Likes)
                .Where(v => v.CreatedAt >= since);

            if (preferredCategories.Any())
                query = query.Where(v => preferredCategories.Contains(v.Category.Name));

            return await query
                .OrderByDescending(v => v.ViewCount + (v.Likes.Count * 2))
                .Take(20)
                .ToListAsync();
        }

        public async Task<IEnumerable<Video>> GetDriftAsync(int userId, IEnumerable<string> watchedCategoryNames)
        {
            // Get videos from categories user hasn't watched
            return await _context.Videos
                .Include(v => v.User)
                .Include(v => v.Category)
                .Where(v => !watchedCategoryNames.Contains(v.Category.Name))
                .Where(v => !_context.WatchHistories.Any(w => w.UserId == userId && w.VideoId == v.Id))
                .OrderBy(v => Guid.NewGuid())
                .Take(20)
                .ToListAsync();
        }

    }
}
