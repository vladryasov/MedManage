using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedManage.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt received, token length: {Len}", request.Token.Length);

        try
        {
            var userDto = await _authService.LoginAsync(request.Token);
            _logger.LogInformation("Login successful for user {UserId}", userDto.UserId);
            return Ok(userDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }
}
