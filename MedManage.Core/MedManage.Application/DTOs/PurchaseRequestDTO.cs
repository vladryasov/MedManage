using MedManage.Domain.Enums;

namespace MedManage.Application.DTOs;

/// <summary>
/// DTO для запроса на покупку.
/// </summary>
public class PurchaseRequestDTO
{
    public Guid PurchaseRequestId { get; set; }
    public Guid? AnnouncementId { get; set; }
    public Guid BuyerUserId { get; set; }
    public Guid SellerUserId { get; set; }
    public PurchaseRequestStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public string? AnnouncementTitle { get; set; }
    public string BuyerUserName { get; set; } = null!;
    public string SellerUserName { get; set; } = null!;
}
