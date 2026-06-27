using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedManage.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.FindByUserNameAsync(request.UserName);
        if (user == null)
        {
            _logger.LogWarning("Login failed: user {UserName} not found", request.UserName);
            throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            _logger.LogWarning("Login failed: user {UserName} has no password set", request.UserName);
            throw new UnauthorizedAccessException("Пароль не установлен. Обратитесь к администратору.");
        }

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: invalid password for {UserName}", request.UserName);
            throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("name", user.UserName)
        };

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken(claims);

        await StoreRefreshTokenAsync(user.UserId, refreshToken);

        var userDto = _mapper.Map<UserDTO>(user);
        _logger.LogInformation("Login successful for user {UserId}", user.UserId);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = userDto
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var principal = _tokenService.ValidateRefreshToken(refreshToken);
        if (principal == null)
        {
            _logger.LogWarning("Refresh failed: invalid token");
            throw new UnauthorizedAccessException("Недействительный токен обновления");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Недействительный токен обновления");
        }

        var tokenHash = HashToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
        if (storedToken == null || !storedToken.IsActive)
        {
            _logger.LogWarning("Refresh failed: token revoked or not found for user {UserId}", userId);
            throw new UnauthorizedAccessException("Недействительный токен обновления");
        }

        await _refreshTokenRepository.RevokeAsync(storedToken.Id);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Пользователь не найден");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("name", user.UserName)
        };

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken(claims);

        await StoreRefreshTokenAsync(user.UserId, newRefreshToken);

        var userDto = _mapper.Map<UserDTO>(user);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = userDto
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
        if (storedToken != null)
        {
            await _refreshTokenRepository.RevokeAsync(storedToken.Id);
            _logger.LogInformation("Refresh token revoked for user {UserId}", storedToken.UserId);
        }
    }

    private async Task StoreRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var expirationDays = int.TryParse(
            _configuration["Jwt:RefreshTokenExpirationDays"], out var days)
            ? days : 7;

        var tokenHash = HashToken(refreshToken);
        var entity = new RefreshToken(
            userId,
            tokenHash,
            DateTime.UtcNow.AddDays(expirationDays));

        await _refreshTokenRepository.AddAsync(entity);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
