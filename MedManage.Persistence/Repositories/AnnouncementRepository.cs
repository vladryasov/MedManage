using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью объявлений.
/// </summary>
public class AnnouncementRepository : IAnnouncementRepository
{
    private const int RecentAnnouncementsLimit = 20;

    private readonly IAppDbContext _context;

    public AnnouncementRepository(IAppDbContext context)
    {
        _context = context;
    }

    [Cache("RecentAnnouncements", ExpirationSeconds = 300)] // 5 минут
    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        return await _context.Announcements
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Take(RecentAnnouncementsLimit)
            .ToListAsync();
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements")]
    public async Task<Announcement> GetByIdAsync(Guid announcementId)
    {
        var announcement = await _context.Announcements
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);

        if (announcement != null)
        {
            announcement.Views++;
            await _context.SaveChangesAsync();
        }

        return announcement;
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")] // сбрасываем список и все кешированные объявления
    public IQueryable<Announcement> GetPaginated(
        int pageNumber,
        int pageSize,
        TypeOfSort sortBy,
        string searchFilter,
        ProductType productType,
        InventoryStatus inventoryStatus)
    {
        var announcements = _context.Announcements
            .Include(a => a.User)
            .AsQueryable();

        if (productType != ProductType.All)
            announcements = announcements.Where(a => a.TypeProduct == productType);

        if (inventoryStatus != InventoryStatus.All)
            announcements = announcements.Where(a => a.StatusInventory == inventoryStatus);

        if (!string.IsNullOrWhiteSpace(searchFilter))
            announcements = announcements.Where(a =>
                a.Title.Contains(searchFilter) || a.Content.Contains(searchFilter));

        announcements = sortBy switch
        {
            TypeOfSort.ByCategory => announcements.OrderByDescending(a => a.StatusInventory),
            TypeOfSort.ByDate => announcements.OrderByDescending(a => a.CreatedAt),
            _ => announcements.OrderByDescending(a => a.CreatedAt)
        };

        var paginatedAnnouncements = announcements
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        foreach (var announcement in paginatedAnnouncements)
        {
            announcement.Views++;
            _context.Announcements.Update(announcement);
        }

        _context.SaveChanges();

        return announcements
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")]
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

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")]
    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

    [Transactional]
    [CacheInvalidate("RecentAnnouncements", "ById:*")]
    public async Task DeleteAsync(Announcement announcement)
    {
        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByAuthorAsync(string authorName)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .Where(a => a.User.FullName.Contains(authorName))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByDateAsync(DateTime date)
    {
        return await _context.Announcements
            .Where(a => a.CreatedAt.Date == date.Date)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> SearchAnnouncementsByContentAsync(string content)
    {
        return await _context.Announcements
            .Where(a => a.Content.Contains(content))
            .ToListAsync();
    }
}