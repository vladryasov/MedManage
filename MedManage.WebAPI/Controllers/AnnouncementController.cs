using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MedManage.Application.Filters;
using Microsoft.AspNetCore.Authorization;


namespace MedManage.WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с объявлениями.
    /// </summary>
    [ApiController]
    [Authorize]  // Требуется авторизация
    [ValidateModelState] // Валидация модели
    [Route("api/[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        /// <summary>
        /// Конструктор контроллера <see cref="AnnouncementController"/>.
        /// </summary>
        /// <param name="announcementService">Сервис для работы с объявлениями.</param>
        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        /// <summary>
        /// Получить все объявления.
        /// </summary>
        /// <returns>Список всех объявлений.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAnnouncementsAsync()
        {
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            return Ok(announcements);
        }

        /// <summary>
        /// Получить объявление по ID.
        /// </summary>
        /// <param name="announcementId">Идентификатор объявления.</param>
        /// <returns>Объявление с указанным ID.</returns>
        [HttpGet("{announcementId}")]
        public async Task<IActionResult> GetAnnouncementById(Guid announcementId)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(announcementId);
            if (announcement == null)
            {
                return NotFound(new { message = "Announcement not found." });
            }
            return Ok(announcement);
        }

        /// <summary>
        /// Получить все объявления с пагинацией.
        /// </summary>
        /// <param name="pageNumber">Номер страницы.</param>
        /// <param name="pageSize">Размер страницы.</param>
        /// <param name="sortBy">Тип сортировки.</param>
        /// <param name="searchFilter">Фильтр для поиска.</param>
        /// <param name="productType">Тип продукта.</param>
        /// <param name="statusInventory">Статус инвентаря.</param>
        /// <param name="views">Статус инвентаря.</param>
        /// <returns>Список объявлений с пагинацией.</returns>
        [HttpGet("paginated")]
        public async Task<IActionResult> GetAllAnnouncementsPaginatedAsync(
            int pageNumber, 
            int pageSize, 
            TypeOfSort sortBy, 
            string searchFilter, 
            ProductType productType,
            InventoryStatus statusInventory,
            int views)
        {
            var announcements = await _announcementService.GetAllAnnouncementsPaginatedAsync(
                pageNumber, 
                pageSize, 
                sortBy, 
                searchFilter, 
                productType, 
                statusInventory,
                views
            );
            return Ok(announcements);
        }

        /// <summary>
        /// Создать новое объявление.
        /// </summary>
        /// <param name="announcement">Данные объявления для создания.</param>
        /// <returns>Результат создания объявления.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewAnnouncementAsync([FromBody] AnnouncementDTO announcement)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Console.WriteLine($"Token: {token}");

            Console.WriteLine($"Extracted UserIdClaim: {userIdClaim}");
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid UserId");
            } 
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"ClaimType: {claim.Type}, ClaimValue: {claim.Value}");
            }

            
            announcement.AnnouncementId = Guid.NewGuid();
            await _announcementService.CreateNewAnnouncementAsync(announcement, userId);
            return CreatedAtAction(nameof(GetAnnouncementById), new { announcementId = announcement.AnnouncementId }, announcement);
        }

        /// <summary>
        /// Изменить содержание объявления.
        /// </summary>
        /// <param name="announcementId">Идентификатор объявления.</param>
        /// <param name="content">Новый контент объявления.</param>
        /// <returns>Результат изменения.</returns>
        [HttpPatch("{announcementId}")]
        public async Task<IActionResult> ChangeAnnouncementContentAsync(Guid announcementId, [FromBody] string content)
        {
            await _announcementService.ChangeAnnouncementContentAsync(announcementId, content);
            return NoContent();
        }

        /// <summary>
        /// Удалить объявление.
        /// </summary>
        /// <param name="announcementId">Идентификатор объявления.</param>
        /// <returns>Результат удаления.</returns>
        [HttpDelete("{announcementId}")]
        public async Task<IActionResult> DeleteAnnouncementAsync(Guid announcementId)
        {
            await _announcementService.DeleteAnnouncementAsync(announcementId);
            return NoContent();
        }
    }
}
