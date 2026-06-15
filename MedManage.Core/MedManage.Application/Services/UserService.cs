using System.IdentityModel.Tokens.Jwt;
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
        public async Task<IEnumerable<UserDTO>> GetAllUsersExceptAsync(Guid userId)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var filteredUsers = users.Where(u => u.UserId != userId);

            return _mapper.Map<IEnumerable<UserDTO>>(filteredUsers);
        }

        /// <inheritdoc />
        public async Task UpdateUserInfoAsync(UserDTO updatedUser)
        {
            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            var existingUser = await _userRepository.GetByIdAsync(updatedUser.Id);
            if (existingUser == null)
                throw new InvalidOperationException("Пользователь не найден.");

            _mapper.Map(updatedUser, existingUser);
            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<UserDTO> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return _mapper.Map<UserDTO>(user);
        }
        
        public async Task UpdateUserRoleAsync(UserDTO updatedUser, UserRole newRole)
        {
            if (updatedUser == null) throw new ArgumentNullException(nameof(updatedUser));
            var existingUser = await _userRepository.GetByIdAsync(updatedUser.Id);
            existingUser.Role = newRole;
            await _userRepository.UpdateAsync(existingUser);
        }
        
        public async Task UpdateUserPhoneNumberAsync(UserDTO updatedUser)
        {
            if (updatedUser == null)  throw new ArgumentNullException(nameof(updatedUser));
            var existingUser = await _userRepository.GetByIdAsync(updatedUser.Id);
            if (existingUser == null)  throw new InvalidOperationException($"{updatedUser.Id} - такого пользователя нет.");
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            await _userRepository.UpdateAsync(existingUser);
        }
        
        
        /// <inheritdoc />
        public string GetUserNameFromToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Токен отсутствует.");

            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            var nameClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            if (string.IsNullOrEmpty(nameClaim))
                throw new InvalidOperationException("Имя пользователя отсутствует в токене.");

            return nameClaim;
        }
    }
}
