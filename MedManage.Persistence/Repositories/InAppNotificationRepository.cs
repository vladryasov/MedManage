using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

public class InAppNotificationRepository : IInAppNotificationRepository
{
    private readonly IAppDbContext _context;

    public InAppNotificationRepository(IAppDbContext context)
    {
        _context = context;
    }

    [Transactional]
    [CacheInvalidate("Notifications:*", "UnreadCount:*")]
    public async Task<InAppNotification> CreateAsync(InAppNotification notification)
    {
        await _context.InAppNotifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var created = await _context.InAppNotifications
            .Include(n => n.SenderUser)
            .Include(n => n.RecipientUser)
            .FirstAsync(n => n.Id == notification.Id);

        return created;
    }

    [Cache("Notifications:{recipientUserId}", ExpirationSeconds = 60)]
    public async Task<IEnumerable<InAppNotification>> GetByRecipientAsync(Guid recipientUserId)
    {
        return await _context.InAppNotifications
            .Include(n => n.SenderUser)
            .Where(n => n.RecipientUserId == recipientUserId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    [Cache("UnreadCount:{recipientUserId}", ExpirationSeconds = 30)]
    public async Task<int> GetUnreadCountAsync(Guid recipientUserId)
    {
        return await _context.InAppNotifications
            .CountAsync(n => n.RecipientUserId == recipientUserId && !n.IsRead);
    }

    public async Task<InAppNotification?> GetByIdAsync(Guid notificationId)
    {
        return await _context.InAppNotifications
            .Include(n => n.SenderUser)
            .FirstOrDefaultAsync(n => n.Id == notificationId);
    }

    [Transactional]
    [CacheInvalidate("Notifications:*", "UnreadCount:*")]
    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _context.InAppNotifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);
        if (notification != null)
        {
            notification.MarkAsRead();
            await _context.SaveChangesAsync();
        }
    }

    [Transactional]
    [CacheInvalidate("Notifications:*", "UnreadCount:*")]
    public async Task MarkAllAsReadAsync(Guid recipientUserId)
    {
        var unread = await _context.InAppNotifications
            .Where(n => n.RecipientUserId == recipientUserId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unread)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync();
    }
}
