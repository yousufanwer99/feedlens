using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace FeedLens.Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FeedLensDbContext _context;

        public UserRepository(FeedLensDbContext context) => _context = context;

        public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

        public async Task<User> CreateAsync(User user)
        {
            user.Email = user.Email.ToLower();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
