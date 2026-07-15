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

    }
}
