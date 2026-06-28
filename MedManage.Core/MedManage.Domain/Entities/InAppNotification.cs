using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

public class InAppNotification
{
    private InAppNotification() { }

    public InAppNotification(
        Guid recipientUserId,
        string title,
        string message,
        InAppNotificationType type,
        Guid? senderUserId = null,
        Guid? relatedEntityId = null)
    {
        Id = Guid.NewGuid();
        RecipientUserId = recipientUserId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = false;
        CreatedAt = DateTimeOffset.UtcNow;
        SenderUserId = senderUserId;
        RelatedEntityId = relatedEntityId;
    }

    public Guid Id { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public Guid? SenderUserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public InAppNotificationType Type { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public User RecipientUser { get; set; } = null!;
    public User? SenderUser { get; set; }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
