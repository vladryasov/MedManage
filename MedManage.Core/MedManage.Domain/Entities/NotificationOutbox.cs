using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

public class NotificationOutbox
{
    private NotificationOutbox() { }

    public NotificationOutbox(
        string recipientEmail,
        string subject,
        string body,
        NotificationType type,
        Guid? recipientUserId = null)
    {
        Id = Guid.NewGuid();
        RecipientEmail = recipientEmail;
        Subject = subject;
        Body = body;
        Type = type;
        Status = NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        RetryCount = 0;
        RecipientUserId = recipientUserId;
    }

    public Guid Id { get; private set; }
    public string RecipientEmail { get; private set; } = null!;
    public Guid? RecipientUserId { get; private set; }
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }

    public User? RecipientUser { get; private set; }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkFailed(string error)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = error;
        RetryCount++;
    }
}
