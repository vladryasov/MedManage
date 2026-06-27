using System.Security.Claims;
using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MedManage.Application.Services;

public class InAppNotificationService : IInAppNotificationService
{
    private readonly IInAppNotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InAppNotificationService(
        IInAppNotificationRepository notificationRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<InAppNotificationDTO>> GetNotificationsAsync()
    {
        var currentUserId = GetCurrentUserId();
        var notifications = await _notificationRepository.GetByRecipientAsync(currentUserId);
        return _mapper.Map<IEnumerable<InAppNotificationDTO>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var currentUserId = GetCurrentUserId();
        return await _notificationRepository.GetUnreadCountAsync(currentUserId);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var currentUserId = GetCurrentUserId();
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
            throw new InvalidOperationException("Уведомление не найдено.");
        if (notification.RecipientUserId != currentUserId)
            throw new UnauthorizedAccessException("Это уведомление не для вас.");
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    public async Task MarkAllAsReadAsync()
    {
        var currentUserId = GetCurrentUserId();
        await _notificationRepository.MarkAllAsReadAsync(currentUserId);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user");
        return userId;
    }
}
