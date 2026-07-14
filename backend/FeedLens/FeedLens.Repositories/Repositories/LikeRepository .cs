using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace FeedLens.Repositories.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly FeedLensDbContext _context;

        public LikeRepository(FeedLensDbContext context) => _context = context;

        public async Task<Like?> GetAsync(int userId, int videoId) =>
            await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.VideoId == videoId);

        public async Task<int> GetCountAsync(int videoId) =>
            await _context.Likes.CountAsync(l => l.VideoId == videoId);

        public async Task<Like> CreateAsync(Like like)
        {
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task DeleteAsync(int userId, int videoId)
        {
            var like = await GetAsync(userId, videoId);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
    }
}
