using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MedManage.Application.Filters;
using MedManage.Domain.Enums;

namespace MedManage.WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с пользователями.
    /// </summary>
    [ApiController]
    [ValidateModelState] // Валидация модели
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Конструктор контроллера <see cref="UserController"/>.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("CurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                var user = await _userService.GetCurrentUserAsync(userId);
                return Ok(user);
            }

            return Unauthorized("Invalid user(current_user)");
        }
        
        /// <summary>
        /// Получить всех пользователей, кроме указанного.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, которого нужно исключить.</param>
        /// <returns>Список пользователей.</returns>
        [HttpGet("users/all")]
        public async Task<IActionResult> GetAllUsersExceptAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                var users = await _userService.GetAllUsersExceptAsync(userId);
                return Ok(users);
            }
            else
            {
                return Unauthorized("ВЫ не авторизованы");
            }
        }

        /// <summary>
        /// Обновить информацию о пользователе.
        /// </summary>
        /// <param name="updatedUser">Обновленные данные пользователя.</param>
        /// <returns>Результат обновления.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserInfoAsync([FromBody] UserDTO updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest(new { message = "User data is required." });
            }

            try
            {
                await _userService.UpdateUserInfoAsync(updatedUser);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Получить имя пользователя из токена.
        /// </summary>
        /// <returns>Имя пользователя, извлеченное из токена.</returns>
        [HttpGet("name")]
        public IActionResult GetUserNameFromToken()
        {
            try
            {
                var userName = _userService.GetUserNameFromToken();
                return Ok(new { userName });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        
        [HttpPatch("users/{userId}/Role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserDTO updatedUser, UserRole newRole)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user(updateDescription)");
            }

            await _userService.UpdateUserRoleAsync(updatedUser, newRole);
            return Ok("роль обновлена успешно");
        }
        
        
        [HttpPatch("users/{userId}/updateNumber")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserDTO updatedUser)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user(updateDescription)");
            }
            await _userService.UpdateUserPhoneNumberAsync(updatedUser);
            return Ok("Номер обновлен");
        }
        
    }
}
