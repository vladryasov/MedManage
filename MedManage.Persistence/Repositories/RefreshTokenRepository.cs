using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IAppDbContext _context;

    public RefreshTokenRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(Guid id)
    {
        var token = await _context.RefreshTokens.FindAsync(id);
        if (token != null)
        {
            token.Revoke();
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.Revoke();
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteExpiredOrRevokedAsync(CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .Where(t => t.RevokedAt != null || t.ExpiresAt < DateTime.UtcNow)
            .ExecuteDeleteAsync(ct);
    }
}
