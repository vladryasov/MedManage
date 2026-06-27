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
        try
        {
            var response = await _authService.LoginAsync(request);
            SetAuthCookies(response.AccessToken, response.RefreshToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest? request = null)
    {
        try
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? request?.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { error = "Недействительный токен обновления" });
            }

            var response = await _authService.RefreshTokenAsync(refreshToken);
            SetAuthCookies(response.AccessToken, response.RefreshToken);

            if (request?.RefreshToken != null && Request.Cookies["refresh_token"] == null)
            {
                ClearAuthCookies();
            }

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            ClearAuthCookies();
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request = null)
    {
        var refreshToken = Request.Cookies["refresh_token"] ?? request?.RefreshToken;
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken);
        }

        ClearAuthCookies();
        return Ok(new { message = "Выход выполнен" });
    }

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };

        Response.Cookies.Append("access_token", accessToken, cookieOptions);

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Path = "/api/Auth"
        };

        Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
    }

    private void ClearAuthCookies()
    {
        Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/Auth" });
    }
}
