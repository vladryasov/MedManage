using MedManage.Domain.Common;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Domain.Interfaces;

public interface IAnnouncementRepository
{
    Task<IEnumerable<Announcement>> GetAllAsync();

    Task<Announcement?> GetByIdAsync(Guid announcementId);

    Task<PaginatedResult<Announcement>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        TypeOfSort sortBy,
        string searchFilter,
        ProductType productType,
        InventoryStatus inventoryStatus);

    Task<Announcement> CreateAsync(
        string title,
        string content,
        Guid createdByUserId,
        InventoryStatus statusInventory,
        ProductType typeProduct,
        Guid? organizationId = null,
        DateTimeOffset? expirationDate = null);

    Task IncrementViewsAsync(Guid announcementId);

    Task UpdateAsync(Announcement announcement);

    Task DeleteAsync(Announcement announcement);

    Task<IEnumerable<Announcement>> GetAnnouncementsByAuthorAsync(string authorName);

    Task<IEnumerable<Announcement>> GetAnnouncementsByDateAsync(DateTime date);

    Task<IEnumerable<Announcement>> SearchAnnouncementsByContentAsync(string content);
    Task<IEnumerable<Announcement>> GetByUserIdAsync(Guid userId);
}
