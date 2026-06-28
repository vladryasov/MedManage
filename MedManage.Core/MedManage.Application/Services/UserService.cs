using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;

namespace MedManage.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IOutboxService _outboxService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IOutboxService outboxService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService;
            _outboxService = outboxService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration;
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            if (currentUser == null || currentUser.Role < UserRole.Admin)
                throw new UnauthorizedAccessException("Недостаточно прав для создания пользователя");

            var existingEmail = await _userRepository.FindByEmailAsync(request.Email);
            if (existingEmail != null)
                throw new InvalidOperationException("Пользователь с таким email уже существует");

            var userName = _passwordService.GenerateUserName();
            while (await _userRepository.FindByUserNameAsync(userName) != null)
                userName = _passwordService.GenerateUserName();

            var password = _passwordService.GeneratePassword();
            var passwordHash = _passwordService.Hash(password);

            var user = await _userRepository.AddAsync(
                userName,
                request.FullName,
                request.Email,
                UserRole.CommonUser,
                request.PhoneNumber ?? "",
                passwordHash,
                request.OrganizationId);

            var adminEmail = _configuration["AdminSettings:Email"];
            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                await _outboxService.AddToOutboxAsync(
                    adminEmail,
                    "Создан новый пользователь",
                    $"В системе создан новый пользователь:\nЛогин: {userName}\nEmail: {request.Email}\nПолное имя: {request.FullName}",
                    NotificationType.AdminNotification);
            }

            await _outboxService.AddToOutboxAsync(
                request.Email,
                "Добро пожаловать в MedManage",
                $"Ваш аккаунт создан.\n\nЛогин: {userName}\nПароль: {password}\n\nПриятного пользования!",
                NotificationType.UserCredentials,
                user.UserId);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersExceptAsync()
        {
            var userId = GetCurrentUserId();
            var users = await _userRepository.GetAllUsersAsync();
            var filteredUsers = users.Where(u => u.UserId != userId);

            return _mapper.Map<IEnumerable<UserDTO>>(filteredUsers);
        }

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

        public async Task UpdateUserRoleAsync(Guid userId, UserRole newRole)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            if (currentUser.Role < UserRole.Admin)
                throw new UnauthorizedAccessException("Недостаточно прав для изменения роли");
            if (newRole > currentUser.Role)
                throw new UnauthorizedAccessException("Нельзя назначить роль выше своей");

            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
                throw new InvalidOperationException("Пользователь не найден.");
            existingUser.Role = newRole;
            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task UpdateUserPhoneNumberAsync(Guid userId, string phoneNumber)
        {
            var currentUserId = GetCurrentUserId();
            if (userId != currentUserId)
            {
                var currentUser = await _userRepository.GetByIdAsync(currentUserId);
                if (currentUser.Role < UserRole.Admin)
                    throw new UnauthorizedAccessException("Недостаточно прав для изменения номера");
            }

            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null) throw new InvalidOperationException($"{userId} - такого пользователя нет.");
            existingUser.PhoneNumber = phoneNumber;
            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            if (currentUser.Role < UserRole.Admin)
                throw new UnauthorizedAccessException("Недостаточно прав");

            if (userId == currentUserId)
                throw new InvalidOperationException("Нельзя удалить самого себя");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            await _userRepository.DeleteAsync(user);
        }

        public string GetUserNameFromToken()
        {
            var nameClaim = _httpContextAccessor.HttpContext?.User.FindFirst("name")?.Value;
            if (string.IsNullOrEmpty(nameClaim))
                throw new InvalidOperationException("Имя пользователя отсутствует в токене.");

            return nameClaim;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user");
            return userId;
        }
    }
}
