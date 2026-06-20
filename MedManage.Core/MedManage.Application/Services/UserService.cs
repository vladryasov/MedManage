using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;

namespace MedManage.Application.Services
{
    /// <summary>
    /// Сервис для управления пользователями.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Конструктор сервиса управления пользователями.
        /// </summary>
        public UserService(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDTO>> GetAllUsersExceptAsync()
        {
            var userId = GetCurrentUserId();
            var users = await _userRepository.GetAllUsersAsync();
            var filteredUsers = users.Where(u => u.UserId != userId);

            return _mapper.Map<IEnumerable<UserDTO>>(filteredUsers);
        }

        /// <inheritdoc />
        public async Task UpdateUserInfoAsync(UserDTO updatedUser)
        {
            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            var existingUser = await _userRepository.GetByIdAsync(updatedUser.UserId);
            if (existingUser == null)
                throw new InvalidOperationException("Пользователь не найден.");

            _mapper.Map(updatedUser, existingUser);
            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<UserDTO> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetUserByIdAsync(userId);
            return _mapper.Map<UserDTO>(user);
        }
        
        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user");
            return userId;
        }
        
        public async Task UpdateUserRoleAsync(UserDTO updatedUser, UserRole newRole)
        {
            if (updatedUser == null) throw new ArgumentNullException(nameof(updatedUser));
            
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            if (currentUser.Role < UserRole.Admin)
                throw new UnauthorizedAccessException("Недостаточно прав для изменения роли");
            if (newRole > currentUser.Role)
                throw new UnauthorizedAccessException("Нельзя назначить роль выше своей");
            
            var existingUser = await _userRepository.GetByIdAsync(updatedUser.UserId);
            existingUser.Role = newRole;
            await _userRepository.UpdateAsync(existingUser);
        }
        
        public async Task UpdateUserPhoneNumberAsync(UserDTO updatedUser)
        {
            if (updatedUser == null)  throw new ArgumentNullException(nameof(updatedUser));
            
            var currentUserId = GetCurrentUserId();
            if (updatedUser.UserId != currentUserId)
            {
                var currentUser = await _userRepository.GetByIdAsync(currentUserId);
                if (currentUser.Role < UserRole.Admin)
                    throw new UnauthorizedAccessException("Недостаточно прав для изменения номера");
            }
            
            var existingUser = await _userRepository.GetByIdAsync(updatedUser.UserId);
            if (existingUser == null)  throw new InvalidOperationException($"{updatedUser.UserId} - такого пользователя нет.");
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            await _userRepository.UpdateAsync(existingUser);
        }
        
        /// <inheritdoc />
        public string GetUserNameFromToken()
        {
            var nameClaim = _httpContextAccessor.HttpContext?.User.FindFirst("name")?.Value;
            if (string.IsNullOrEmpty(nameClaim))
                throw new InvalidOperationException("Имя пользователя отсутствует в токене.");

            return nameClaim;
        }
    }
}
