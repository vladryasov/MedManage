using MedManage.Application.DTOs;

namespace MedManage.Application.Interfaces;

public interface IInAppNotificationService
{
    Task<IEnumerable<InAppNotificationDTO>> GetNotificationsAsync();
    Task<int> GetUnreadCountAsync();
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync();
}
