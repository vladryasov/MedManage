using System.Security.Claims;
using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MedManage.Application.Services;

public class PurchaseRequestService : IPurchaseRequestService
{
    private readonly IPurchaseRequestRepository _purchaseRequestRepository;
    private readonly IAnnouncementRepository _announcementRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PurchaseRequestService> _logger;

    public PurchaseRequestService(
        IPurchaseRequestRepository purchaseRequestRepository,
        IAnnouncementRepository announcementRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PurchaseRequestService> logger)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
        _announcementRepository = announcementRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<PurchaseRequestDTO> CreateRequestAsync(CreatePurchaseRequestRequest request)
    {
        var currentUserId = GetCurrentUserId();

        var announcement = await _announcementRepository.GetByIdAsync(request.AnnouncementId);
        if (announcement == null)
        {
            throw new InvalidOperationException("Объявление не найдено.");
        }

        if (announcement.CreatedByUserId == currentUserId)
        {
            throw new InvalidOperationException("Нельзя создать запрос на своё объявление.");
        }

        var currentUser = await _userRepository.GetByIdAsync(currentUserId);

        var purchaseRequest = new PurchaseRequest(
            request.AnnouncementId,
            currentUserId,
            announcement.CreatedByUserId,
            request.Message);

        var notification = new InAppNotification(
            announcement.CreatedByUserId,
            "Новый запрос на покупку",
            $"Пользователь {currentUser.UserName} хочет купить: {announcement.Title}",
            InAppNotificationType.PurchaseRequest,
            currentUserId,
            purchaseRequest.PurchaseRequestId);

        var created = await _purchaseRequestRepository.CreateWithNotificationAsync(purchaseRequest, notification);
        return _mapper.Map<PurchaseRequestDTO>(created);
    }

    public async Task<IEnumerable<PurchaseRequestDTO>> GetIncomingRequestsAsync()
    {
        var currentUserId = GetCurrentUserId();
        var requests = await _purchaseRequestRepository.GetBySellerAsync(currentUserId);
        return _mapper.Map<IEnumerable<PurchaseRequestDTO>>(requests);
    }

    public async Task<IEnumerable<PurchaseRequestDTO>> GetOutgoingRequestsAsync()
    {
        var currentUserId = GetCurrentUserId();
        var requests = await _purchaseRequestRepository.GetByBuyerAsync(currentUserId);
        return _mapper.Map<IEnumerable<PurchaseRequestDTO>>(requests);
    }

    public async Task AcceptRequestAsync(Guid requestId)
    {
        var currentUserId = GetCurrentUserId();

        var request = await _purchaseRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new InvalidOperationException("Запрос не найден.");
        }

        if (request.SellerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Только продавец может принять запрос.");
        }

        if (request.Status != PurchaseRequestStatus.Pending)
        {
            throw new InvalidOperationException("Запрос уже обработан.");
        }

        var announcement = request.Announcement;
        if (announcement == null)
        {
            throw new InvalidOperationException("Объявление не найдено.");
        }

        var notification = new InAppNotification(
            request.BuyerUserId,
            "Запрос на покупку принят",
            $"Пользователь {request.SellerUser.UserName} принял ваш запрос на покупку: {announcement.Title}",
            InAppNotificationType.RequestAccepted,
            currentUserId,
            request.PurchaseRequestId);

        await _purchaseRequestRepository.AcceptRequestAsync(request, announcement, notification);
    }

    public async Task RejectRequestAsync(Guid requestId)
    {
        var currentUserId = GetCurrentUserId();

        var request = await _purchaseRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new InvalidOperationException("Запрос не найден.");
        }

        if (request.SellerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Только продавец может отклонить запрос.");
        }

        if (request.Status != PurchaseRequestStatus.Pending)
        {
            throw new InvalidOperationException("Запрос уже обработан.");
        }

        var announcementTitle = request.Announcement?.Title ?? "товар";
        var notification = new InAppNotification(
            request.BuyerUserId,
            "Запрос на покупку отклонён",
            $"Пользователь {request.SellerUser.UserName} отклонил ваш запрос на покупку: {announcementTitle}",
            InAppNotificationType.RequestRejected,
            currentUserId,
            request.PurchaseRequestId);

        await _purchaseRequestRepository.RejectRequestAsync(request, notification);
    }

    public async Task DeleteRequestAsync(Guid requestId)
    {
        var currentUserId = GetCurrentUserId();

        var request = await _purchaseRequestRepository.GetByIdLightAsync(requestId);
        if (request == null)
        {
            throw new InvalidOperationException("Запрос не найден.");
        }

        if (request.BuyerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Только отправитель может удалить запрос.");
        }

        if (request.Status != PurchaseRequestStatus.Pending)
        {
            throw new InvalidOperationException("Нельзя удалить обработанный запрос.");
        }

        await _purchaseRequestRepository.DeleteRequestAsync(request);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user");
        return userId;
    }
}
