using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using MedManage.Application.Filters;
using Microsoft.AspNetCore.Authorization;

namespace MedManage.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [ValidateModelState]
    [Route("api/[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAnnouncementsAsync()
        {
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            return Ok(announcements);
        }

        [HttpGet("{announcementId}")]
        public async Task<IActionResult> GetAnnouncementById(Guid announcementId)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(announcementId);
            return Ok(announcement);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyAnnouncements()
        {
            var announcements = await _announcementService.GetMyAnnouncementsAsync();
            return Ok(announcements);
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetAllAnnouncementsPaginatedAsync(
            int pageNumber, 
            int pageSize, 
            TypeOfSort sortBy = TypeOfSort.ByDate, 
            string? searchFilter = null, 
            ProductType productType = ProductType.All,
            InventoryStatus statusInventory = InventoryStatus.All)
        {
            var announcements = await _announcementService.GetAllAnnouncementsPaginatedAsync(
                pageNumber, pageSize, sortBy, searchFilter ?? string.Empty, productType, statusInventory);
            return Ok(announcements);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewAnnouncementAsync([FromBody] AnnouncementDTO announcement)
        {
            var created = await _announcementService.CreateNewAnnouncementAsync(announcement);
            return CreatedAtAction(nameof(GetAnnouncementById), new { announcementId = created.AnnouncementId }, created);
        }

        [HttpPatch("{announcementId}")]
        public async Task<IActionResult> ChangeAnnouncementContentAsync(Guid announcementId, [FromBody] UpdateAnnouncementContentRequest request)
        {
            await _announcementService.ChangeAnnouncementContentAsync(announcementId, request.Content);
            return NoContent();
        }

        [HttpDelete("{announcementId}")]
        public async Task<IActionResult> DeleteAnnouncementAsync(Guid announcementId)
        {
            await _announcementService.DeleteAnnouncementAsync(announcementId);
            return NoContent();
        }
    }
}
