using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedManage.Application.Services;

public class OutboxService : IOutboxService
{
    private readonly IAppDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        IAppDbContext context,
        IEmailSender emailSender,
        ILogger<OutboxService> logger)
    {
        _context = context;
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

        _context.NotificationOutbox.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogDebug("Added outbox notification: {Subject} -> {Email}", subject, recipientEmail);
    }

    public async Task ProcessOutboxAsync()
    {
        var pending = await _context.NotificationOutbox
            .Where(n => n.Status == NotificationStatus.Pending)
            .Take(20)
            .ToListAsync();

        foreach (var notification in pending)
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    notification.RecipientEmail,
                    notification.Subject,
                    notification.Body);

                notification.MarkSent();
                _logger.LogInformation("Outbox notification sent: {Id}", notification.Id);
            }
            catch (Exception ex)
            {
                notification.MarkFailed(ex.Message);
                _logger.LogError(ex, "Failed to send outbox notification: {Id}", notification.Id);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task CleanupOutboxAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-7);
        var oldSent = await _context.NotificationOutbox
            .Where(n => n.Status == NotificationStatus.Sent && n.SentAt < cutoff)
            .ToListAsync();

        if (oldSent.Count == 0)
            return;

        _context.NotificationOutbox.RemoveRange(oldSent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cleaned up {Count} old outbox notifications", oldSent.Count);
    }
}
