using MedManage.Domain.Enums;

namespace MedManage.Application.DTOs;

/// <summary>
/// DTO для внутрисистемного уведомления.
/// </summary>
public class InAppNotificationDTO
{
    public Guid Id { get; set; }
    public Guid RecipientUserId { get; set; }
    public Guid? SenderUserId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public InAppNotificationType Type { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? SenderUserName { get; set; }
}
