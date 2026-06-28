using MedManage.Domain.Interfaces;

namespace MedManage.WebAPI.BackgroundServices;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;

    public RefreshTokenCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<RefreshTokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefreshTokenCleanupService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                var deleted = await repository.DeleteExpiredOrRevokedAsync(stoppingToken);

                if (deleted > 0)
                {
                    _logger.LogInformation("Cleaned up {Count} expired/revoked refresh tokens", deleted);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up refresh tokens");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
