using MedManage.Application.DTOs;
using MedManage.Domain.Common;
using MedManage.Domain.Enums;

namespace MedManage.Application.Interfaces;

public interface IAnnouncementService
{
    Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncementsAsync();

    Task<AnnouncementDTO> GetAnnouncementByIdAsync(Guid announcementId);

    Task<PaginatedResult<AnnouncementDTO>> GetAllAnnouncementsPaginatedAsync(
        int pageNumber,
        int pageSize,
        TypeOfSort sortBy,
        string searchFilter,
        ProductType productType,
        InventoryStatus statusInventory);

    Task<AnnouncementDTO> CreateNewAnnouncementAsync(AnnouncementDTO announcementRequest);

    Task ChangeAnnouncementContentAsync(Guid announcementId, string content);

    Task DeleteAnnouncementAsync(Guid announcementId);

    Task<IEnumerable<AnnouncementDTO>> GetMyAnnouncementsAsync();

    Task<IEnumerable<AnnouncementDTO>> GetAnnouncementsByUserIdAsync(Guid userId);

    string GetUserNameFromToken();
}