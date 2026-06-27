using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Domain.Interfaces;

public interface IPurchaseRequestRepository
{
    Task<PurchaseRequest> CreateAsync(PurchaseRequest purchaseRequest);
    Task<PurchaseRequest> CreateWithNotificationAsync(PurchaseRequest purchaseRequest, InAppNotification notification);
    Task<PurchaseRequest?> GetByIdAsync(Guid purchaseRequestId);
    Task<PurchaseRequest?> GetByIdLightAsync(Guid purchaseRequestId);
    Task<IEnumerable<PurchaseRequest>> GetBySellerAsync(Guid sellerUserId);
    Task<IEnumerable<PurchaseRequest>> GetByBuyerAsync(Guid buyerUserId);
    Task UpdateAsync(PurchaseRequest purchaseRequest);
    Task DeleteAsync(PurchaseRequest purchaseRequest);
    Task AcceptRequestAsync(PurchaseRequest request, Announcement announcement, InAppNotification notification);
    Task RejectRequestAsync(PurchaseRequest request, InAppNotification notification);
    Task<bool> DeleteRequestAsync(PurchaseRequest request);
}
