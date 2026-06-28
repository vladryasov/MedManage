using MedManage.Domain.Entities;

namespace MedManage.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task AddAsync(RefreshToken refreshToken);
    Task RevokeAsync(Guid id);
    Task RevokeAllForUserAsync(Guid userId);
    Task<int> DeleteExpiredOrRevokedAsync(CancellationToken ct = default);
}
