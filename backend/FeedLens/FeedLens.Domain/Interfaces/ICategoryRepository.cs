using FeedLens.Domain.Entities;

namespace FeedLens.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllActiveAsync();
    }
}
