using MedManage.Application.Interfaces;

namespace MedManage.WebAPI.BackgroundServices;

public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorService> _logger;

    public OutboxProcessorService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessorService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                await outboxService.ProcessOutboxAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
