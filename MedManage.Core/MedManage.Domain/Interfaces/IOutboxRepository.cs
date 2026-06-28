using MedManage.Domain.Entities;

namespace MedManage.Domain.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(NotificationOutbox notification);
    Task<List<NotificationOutbox>> GetPendingAsync(int take = 20);
    Task UpdateAsync(NotificationOutbox notification);
    Task DeleteOldSentAsync(DateTime cutoff);
}
