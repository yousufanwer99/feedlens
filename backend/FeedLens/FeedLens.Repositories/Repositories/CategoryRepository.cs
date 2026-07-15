using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace FeedLens.Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FeedLensDbContext _context;

        public CategoryRepository(FeedLensDbContext context) => _context = context;

        public async Task<IEnumerable<Category>> GetAllActiveAsync() =>
            await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
    }
}
