using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Services.DTOs;
using FeedLens.Services.Interfaces;

namespace FeedLens.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo) => _categoryRepo = categoryRepo;

        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllAsync()
        {
            try
            {
                var categories = await _categoryRepo.GetAllActiveAsync();
                var dtos = categories.Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon
                });
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Failure($"Failed to load categories: {ex.Message}");
            }
        }
    }
}
