using FeedLens.Services.DTOs;
using FeedLens.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeedLens.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService) => _videoService = videoService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            var result = await _videoService.GetAllAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _videoService.GetByIdAsync(id, userId);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var userId = GetCurrentUserId();
            var result = await _videoService.SearchAsync(query, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyVideos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _videoService.GetMyVideosAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("upload-url")]
        public async Task<IActionResult> GetUploadUrl([FromQuery] string fileName, [FromQuery] string contentType)
        {
            var result = await _videoService.GetUploadUrlAsync(fileName, contentType);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] VideoUploadDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _videoService.UploadAsync(userId, request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _videoService.DeleteAsync(id, userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim) : null;
        }
    }
}
