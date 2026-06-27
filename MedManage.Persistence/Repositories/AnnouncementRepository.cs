using MedManage.Domain.Common;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

public class AnnouncementRepository : IAnnouncementRepository
{
    private const int RecentAnnouncementsLimit = 20;

    private readonly IAppDbContext _context;

    public AnnouncementRepository(IAppDbContext context)
    {
        _context = context;
    }

    [Cache("RecentAnnouncements", ExpirationSeconds = 300)]
    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        return await _context.Announcements
            .Include(a => a.User)
            .Where(NotExpired())
            .OrderByDescending(a => a.CreatedAt)
            .Take(RecentAnnouncementsLimit)
            .ToListAsync();
    }

    [Cache("AnnouncementById:{announcementId}", ExpirationSeconds = 600)]
    public async Task<Announcement?> GetByIdAsync(Guid announcementId)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);
    }

    public async Task<PaginatedResult<Announcement>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        TypeOfSort sortBy,
        string searchFilter,
        ProductType productType,
        InventoryStatus inventoryStatus)
    {
        var query = _context.Announcements
            .Include(a => a.User)
            .Where(NotExpired())
            .AsQueryable();

        if (productType != ProductType.All)
            query = query.Where(a => a.TypeProduct == productType);

        if (inventoryStatus != InventoryStatus.All)
            query = query.Where(a => a.StatusInventory == inventoryStatus);

        if (!string.IsNullOrWhiteSpace(searchFilter))
            query = query.Where(a =>
                a.Title.Contains(searchFilter) || a.Content.Contains(searchFilter));

        query = sortBy switch
        {
            TypeOfSort.ByCategory => query.OrderByDescending(a => a.StatusInventory),
            TypeOfSort.ByDate => query.OrderByDescending(a => a.CreatedAt),
            _ => query.OrderByDescending(a => a.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Announcement>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "AnnouncementById:*")]
    public async Task<Announcement> CreateAsync(
        string title,
        string content,
        Guid createdByUserId,
        InventoryStatus statusInventory,
        ProductType typeProduct,
        Guid? organizationId = null,
        DateTimeOffset? expirationDate = null)
    {
        var announcement = new Announcement(
            title,
            content,
            createdByUserId,
            statusInventory,
            typeProduct,
            organizationId,
            expirationDate);

        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();

        var announcementWithUser = await _context.Announcements
            .Include(a => a.User)
            .FirstAsync(a => a.AnnouncementId == announcement.AnnouncementId);

        return announcementWithUser;
    }

    public async Task IncrementViewsAsync(Guid announcementId)
    {
        await _context.Announcements
            .Where(a => a.AnnouncementId == announcementId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.Views, a => a.Views + 1));
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "AnnouncementById:*")]
    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "AnnouncementById:*")]
    public async Task DeleteAsync(Announcement announcement)
    {
        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByAuthorAsync(string authorName)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .Where(NotExpired())
            .Where(a => a.User.FullName.Contains(authorName))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByDateAsync(DateTime date)
    {
        return await _context.Announcements
            .Where(NotExpired())
            .Where(a => a.CreatedAt.Date == date.Date)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> SearchAnnouncementsByContentAsync(string content)
    {
        return await _context.Announcements
            .Where(NotExpired())
            .Where(a => a.Content.Contains(content))
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .Where(a => a.CreatedByUserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    private static System.Linq.Expressions.Expression<Func<Announcement, bool>> NotExpired()
    {
        return a => a.ExpirationDate == null || a.ExpirationDate > DateTimeOffset.UtcNow;
    }
}