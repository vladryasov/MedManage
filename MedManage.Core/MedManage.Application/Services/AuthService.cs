using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedManage.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserDTO> LoginAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            RequireExpirationTime = false,
        };

        try
        {
            var principal = handler.ValidateToken(token, validationParameters, out _);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Failed to parse sub claim as GUID");
                throw new UnauthorizedAccessException("Недействительный токен");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found in DB: {UserId}", userId);
                throw new UnauthorizedAccessException("Пользователь не найден");
            }

            var userDto = _mapper.Map<UserDTO>(user);

            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value
                            ?? principal.FindFirst("role")?.Value;

            if (Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var jwtRole))
            {
                if (jwtRole != userDto.Role)
                {
                    _logger.LogWarning("Role mismatch: claim role={JwtRole}, DB role={DbRole}", jwtRole, userDto.Role);
                    throw new UnauthorizedAccessException("Несоответствие данных токена");
                }
            }
            else
            {
                var adminClaim = principal.FindFirst("admin")?.Value;
                var jwtAdmin = bool.TryParse(adminClaim, out var admin) && admin;

                if (jwtAdmin != (userDto.Role >= UserRole.Admin))
                {
                    _logger.LogWarning("Role mismatch: admin claim={JwtAdmin}, DB role >= Admin={DbAdmin}", jwtAdmin, userDto.Role >= UserRole.Admin);
                    throw new UnauthorizedAccessException("Несоответствие данных токена");
                }
            }

            _logger.LogInformation("Login successful for user {UserId}", userId);
            return userDto;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JWT validation failed: {Message}", ex.Message);
            throw new UnauthorizedAccessException("Недействительный токен");
        }
    }
}
