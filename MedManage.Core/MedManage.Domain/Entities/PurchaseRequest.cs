using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

public class PurchaseRequest
{
    private PurchaseRequest() { }

    public PurchaseRequest(
        Guid announcementId,
        Guid buyerUserId,
        Guid sellerUserId,
        string? message = null)
    {
        PurchaseRequestId = Guid.NewGuid();
        AnnouncementId = announcementId;
        BuyerUserId = buyerUserId;
        SellerUserId = sellerUserId;
        Message = message;
        Status = PurchaseRequestStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid PurchaseRequestId { get; private set; }
    public Guid? AnnouncementId { get; set; }
    public Guid BuyerUserId { get; private set; }
    public Guid SellerUserId { get; private set; }
    public PurchaseRequestStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Announcement? Announcement { get; set; }
    public User BuyerUser { get; set; } = null!;
    public User SellerUser { get; set; } = null!;
}
