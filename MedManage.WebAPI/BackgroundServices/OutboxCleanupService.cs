using MedManage.Application.Interfaces;

namespace MedManage.WebAPI.BackgroundServices;

public class OutboxCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxCleanupService> _logger;

    public OutboxCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxCleanupService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                await outboxService.CleanupOutboxAsync();

                _logger.LogInformation("Outbox cleanup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up outbox");
            }

            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
        }
    }
}
