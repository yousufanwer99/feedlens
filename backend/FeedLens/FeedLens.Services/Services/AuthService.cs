using FeedLens.Domain.Entities;
using FeedLens.Domain.Interfaces;
using FeedLens.Helpers;
using FeedLens.Services.DTOs;
using FeedLens.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FeedLens.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var existing = await _userRepo.GetByEmailAsync(request.Email);
                if (existing != null)
                    return ApiResponse<AuthResponseDto>.Failure("Email already registered");

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                var created = await _userRepo.CreateAsync(user);
                var token = GenerateToken(created);

                return ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = created.Id,
                    FullName = created.FullName,
                    Email = created.Email,
                    Token = token,
                    Role = created.Role.ToString()
                }, "Registration successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"Registration failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _userRepo.GetByEmailAsync(request.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return ApiResponse<AuthResponseDto>.Failure("Invalid email or password");

                var token = GenerateToken(user);

                return ApiResponse<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Token = token,
                    Role = user.Role.ToString()
                }, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.Failure($"Login failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<UserProfileDto>.Failure("User not found");

                return ApiResponse<UserProfileDto>.Success(MapToProfileDto(user));
            }
            catch (Exception ex)
            {
                return ApiResponse<UserProfileDto>.Failure($"Failed to get profile: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto request)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<UserProfileDto>.Failure("User not found");

                if (request.FullName != null) user.FullName = request.FullName;
                if (request.Bio != null) user.Bio = request.Bio;
                if (request.PreferredCategories != null) user.PreferredCategories = request.PreferredCategories;
                if (request.AvoidCategories != null) user.AvoidCategories = request.AvoidCategories;
                if (request.AlgorithmMode != null) user.AlgorithmMode = request.AlgorithmMode;

                var updated = await _userRepo.UpdateAsync(user);
                return ApiResponse<UserProfileDto>.Success(MapToProfileDto(updated), "Profile updated");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserProfileDto>.Failure($"Failed to update profile: {ex.Message}");
            }
        }
        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static UserProfileDto MapToProfileDto(User user) => new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            PreferredCategories = user.PreferredCategories,
            AvoidCategories = user.AvoidCategories,
            CreatedAt = user.CreatedAt,
            AlgorithmMode = user.AlgorithmMode
        };
    }
}
