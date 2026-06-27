using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManage.Application.Filters;
using MedManage.Domain.Enums;

namespace MedManage.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [ValidateModelState]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("CurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userService.GetCurrentUserAsync();
            return Ok(user);
        }

        [HttpGet("users/all")]
        public async Task<IActionResult> GetAllUsersExceptAsync()
        {
            var users = await _userService.GetAllUsersExceptAsync();
            return Ok(users);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserInfoAsync([FromBody] UserDTO updatedUser)
        {
            await _userService.UpdateUserInfoAsync(updatedUser);
            return NoContent();
        }

        [HttpGet("name")]
        public IActionResult GetUserNameFromToken()
        {
            var userName = _userService.GetUserNameFromToken();
            return Ok(new { userName });
        }

        [HttpPatch("{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid userId, UserRole newRole)
        {
            await _userService.UpdateUserRoleAsync(userId, newRole);
            return Ok("роль обновлена успешно");
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{userId}/phone")]
        public async Task<IActionResult> UpdateUserPhoneNumber(Guid userId, [FromBody] string phoneNumber)
        {
            await _userService.UpdateUserPhoneNumberAsync(userId, phoneNumber);
            return Ok("Номер обновлен");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
