using MedManage.Domain.Enums;

namespace MedManage.Application.Interfaces;

public interface IOutboxService
{
    Task AddToOutboxAsync(string recipientEmail, string subject, string body, NotificationType type, Guid? recipientUserId = null);
    Task ProcessOutboxAsync();
    Task CleanupOutboxAsync();
}
