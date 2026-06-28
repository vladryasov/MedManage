using System.Security.Claims;
using MedManage.Application.DTOs;

namespace MedManage.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    ClaimsPrincipal? ValidateRefreshToken(string token);
}
