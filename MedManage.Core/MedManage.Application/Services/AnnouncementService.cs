using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Application.Exceptions;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MedManage.Domain.Common;

namespace MedManage.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IMapper _mapper;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public AnnouncementService(
            IMapper mapper,
            IAnnouncementRepository announcementRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _announcementRepository = announcementRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncementsAsync()
        {
            var announcements = await _announcementRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AnnouncementDTO>>(announcements);
        }

        public async Task<AnnouncementDTO> GetAnnouncementByIdAsync(Guid announcementId)
        {
            await _announcementRepository.IncrementViewsAsync(announcementId);

            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null)
                throw new AnnouncementNotFoundException($"Объявление с ID {announcementId} не найдено.");

            return _mapper.Map<AnnouncementDTO>(announcement);
        }

        public async Task<PaginatedResult<AnnouncementDTO>> GetAllAnnouncementsPaginatedAsync(
            int pageNumber,
            int pageSize,
            TypeOfSort sortBy,
            string searchFilter,
            ProductType productType,
            InventoryStatus statusInventory)
        {
            var result = await _announcementRepository.GetPaginatedAsync(
                pageNumber,
                pageSize,
                sortBy,
                searchFilter,
                productType,
                statusInventory);

            return new PaginatedResult<AnnouncementDTO>
            {
                Items = _mapper.Map<List<AnnouncementDTO>>(result.Items),
                TotalCount = result.TotalCount
            };
        }

        public async Task<AnnouncementDTO> CreateNewAnnouncementAsync(AnnouncementDTO announcementRequest)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid UserId");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("Пользователь не найден.");

            if (announcementRequest.ExpirationDate.HasValue && announcementRequest.ExpirationDate.Value <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Дата истечения должна быть в будущем.");

            var created = await _announcementRepository.CreateAsync(
                announcementRequest.Title,
                announcementRequest.Content,
                userId,
                announcementRequest.StatusInventory,
                announcementRequest.TypeProduct,
                user.OrganizationId,
                announcementRequest.ExpirationDate);

            return _mapper.Map<AnnouncementDTO>(created);
        }

        public async Task ChangeAnnouncementContentAsync(Guid announcementId, string content)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null)
                throw new AnnouncementNotFoundException($"Объявление с ID {announcementId} не найдено.");

            announcement.Content = content;
            announcement.UpdatedAt = DateTime.UtcNow;

            await _announcementRepository.UpdateAsync(announcement);
        }

        public async Task<IEnumerable<AnnouncementDTO>> GetMyAnnouncementsAsync()
        {
            var userId = GetCurrentUserId();
            return await GetAnnouncementsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<AnnouncementDTO>> GetAnnouncementsByUserIdAsync(Guid userId)
        {
            var announcements = await _announcementRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AnnouncementDTO>>(announcements);
        }

        public async Task DeleteAnnouncementAsync(Guid announcementId)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null)
                throw new AnnouncementNotFoundException($"Объявление с ID {announcementId} не найдено.");

            await _announcementRepository.DeleteAsync(announcement);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user");
            return userId;
        }

        public string GetUserNameFromToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                throw new InvalidOperationException("Пользователь не аутентифицирован.");

            var token = httpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Токен авторизации отсутствует.");

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name");
            if (nameClaim == null)
                throw new InvalidOperationException("Поле 'name' отсутствует в токене.");

            return nameClaim.Value;
        }
    }
}
