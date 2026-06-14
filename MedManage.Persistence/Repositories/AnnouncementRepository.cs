using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью объявлений.
/// </summary>
public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly MedManageDbContext _context;

    public AnnouncementRepository(MedManageDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        return await _context.Announcements
            .Include(a => a.User)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Announcement> GetByIdAsync(Guid announcementId)
    {
        return await _context.Announcements
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnnouncementId == announcementId);
    }

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
        {
            announcements = announcements.Where(a => a.TypeProduct == productType);
        }

        if (inventoryStatus != InventoryStatus.All)
        {
            announcements = announcements.Where(a => a.StatusInventory == inventoryStatus);
        }

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            announcements = announcements.Where(a =>
                a.Title.Contains(searchFilter) || a.Content.Contains(searchFilter));
        }

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

    public async Task CreateAsync(Announcement announcement)
    {
        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

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
