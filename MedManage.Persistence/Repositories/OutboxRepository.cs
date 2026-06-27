using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly IAppDbContext _context;

    public OutboxRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NotificationOutbox notification)
    {
        await _context.NotificationOutbox.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<List<NotificationOutbox>> GetPendingAsync(int take = 20)
    {
        return await _context.NotificationOutbox
            .Where(n => n.Status == NotificationStatus.Pending)
            .Take(take)
            .ToListAsync();
    }

    public async Task UpdateAsync(NotificationOutbox notification)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOldSentAsync(DateTime cutoff)
    {
        var oldSent = await _context.NotificationOutbox
            .Where(n => n.Status == NotificationStatus.Sent && n.SentAt < cutoff)
            .ToListAsync();

        if (oldSent.Count == 0)
            return;

        _context.NotificationOutbox.RemoveRange(oldSent);
        await _context.SaveChangesAsync();
    }
}
