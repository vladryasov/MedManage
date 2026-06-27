using MedManage.Domain.Entities;

namespace MedManage.Domain.Interfaces;

public interface IInAppNotificationRepository
{
    Task<InAppNotification> CreateAsync(InAppNotification notification);
    Task<IEnumerable<InAppNotification>> GetByRecipientAsync(Guid recipientUserId);
    Task<int> GetUnreadCountAsync(Guid recipientUserId);
    Task<InAppNotification?> GetByIdAsync(Guid notificationId);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid recipientUserId);
}
