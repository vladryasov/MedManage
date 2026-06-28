using MedManage.Application.DTOs;

namespace MedManage.Application.Interfaces;

public interface IPurchaseRequestService
{
    Task<PurchaseRequestDTO> CreateRequestAsync(CreatePurchaseRequestRequest request);
    Task<IEnumerable<PurchaseRequestDTO>> GetIncomingRequestsAsync();
    Task<IEnumerable<PurchaseRequestDTO>> GetOutgoingRequestsAsync();
    Task AcceptRequestAsync(Guid requestId);
    Task RejectRequestAsync(Guid requestId);
    Task DeleteRequestAsync(Guid requestId);
}
