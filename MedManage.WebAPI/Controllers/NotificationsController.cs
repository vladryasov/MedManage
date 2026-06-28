using MedManage.Application.Filters;
using MedManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedManage.WebAPI.Controllers;

[ApiController]
[Authorize]
[ValidateModelState]
[Route("api/Notification")]
public class NotificationsController : ControllerBase
{
    private readonly IInAppNotificationService _notificationService;

    public NotificationsController(IInAppNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _notificationService.GetNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync();
        return Ok(new { count });
    }

    [HttpPatch("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        await _notificationService.MarkAsReadAsync(notificationId);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _notificationService.MarkAllAsReadAsync();
        return NoContent();
    }
}
