using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedManage.Application.Services;

public class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        IOutboxRepository outboxRepository,
        IEmailSender emailSender,
        ILogger<OutboxService> logger)
    {
        _outboxRepository = outboxRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task AddToOutboxAsync(
        string recipientEmail,
        string subject,
        string body,
        NotificationType type,
        Guid? recipientUserId = null)
    {
        var notification = new NotificationOutbox(
            recipientEmail, subject, body, type, recipientUserId);

        await _outboxRepository.AddAsync(notification);

        _logger.LogDebug("Added outbox notification: {Subject} -> {Email}", subject, recipientEmail);
    }

    public async Task ProcessOutboxAsync()
    {
        var pending = await _outboxRepository.GetPendingAsync(20);

        foreach (var notification in pending)
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    notification.RecipientEmail,
                    notification.Subject,
                    notification.Body);

                notification.MarkSent();
                await _outboxRepository.UpdateAsync(notification);
                _logger.LogInformation("Outbox notification sent: {Id}", notification.Id);
            }
            catch (Exception ex)
            {
                notification.MarkFailed(ex.Message);
                await _outboxRepository.UpdateAsync(notification);
                _logger.LogError(ex, "Failed to send outbox notification: {Id}", notification.Id);
            }
        }
    }

    public async Task CleanupOutboxAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-7);
        await _outboxRepository.DeleteOldSentAsync(cutoff);
        _logger.LogInformation("Cleaned up old outbox notifications");
    }
}
