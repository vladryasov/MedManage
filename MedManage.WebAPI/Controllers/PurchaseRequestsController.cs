using MedManage.Application.DTOs;
using MedManage.Application.Filters;
using MedManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedManage.WebAPI.Controllers;

[ApiController]
[Authorize]
[ValidateModelState]
[Route("api/PurchaseRequest")]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IPurchaseRequestService _purchaseRequestService;

    public PurchaseRequestsController(IPurchaseRequestService purchaseRequestService)
    {
        _purchaseRequestService = purchaseRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreatePurchaseRequestRequest request)
    {
        var created = await _purchaseRequestService.CreateRequestAsync(request);
        return Ok(created);
    }

    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncomingRequests()
    {
        var requests = await _purchaseRequestService.GetIncomingRequestsAsync();
        return Ok(requests);
    }

    [HttpGet("outgoing")]
    public async Task<IActionResult> GetOutgoingRequests()
    {
        var requests = await _purchaseRequestService.GetOutgoingRequestsAsync();
        return Ok(requests);
    }

    [HttpPatch("{requestId}/accept")]
    public async Task<IActionResult> AcceptRequest(Guid requestId)
    {
        await _purchaseRequestService.AcceptRequestAsync(requestId);
        return Ok("Запрос принят, объявление удалено.");
    }

    [HttpPatch("{requestId}/reject")]
    public async Task<IActionResult> RejectRequest(Guid requestId)
    {
        await _purchaseRequestService.RejectRequestAsync(requestId);
        return Ok("Запрос отклонён.");
    }

    [HttpDelete("{requestId}")]
    public async Task<IActionResult> DeleteRequest(Guid requestId)
    {
        await _purchaseRequestService.DeleteRequestAsync(requestId);
        return Ok("Запрос удалён.");
    }
}
