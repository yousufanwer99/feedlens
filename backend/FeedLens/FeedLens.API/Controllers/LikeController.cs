using FeedLens.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeedLens.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService) => _likeService = likeService;

        [HttpPost("{videoId}")]
        public async Task<IActionResult> ToggleLike(int videoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _likeService.ToggleLikeAsync(userId, videoId);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
    }
}
