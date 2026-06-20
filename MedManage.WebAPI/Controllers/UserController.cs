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
        
        [HttpPatch("users/{userId}/Role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserDTO updatedUser, UserRole newRole)
        {
            await _userService.UpdateUserRoleAsync(updatedUser, newRole);
            return Ok("роль обновлена успешно");
        }
        
        [HttpPatch("users/{userId}/updateNumber")]
        public async Task<IActionResult> UpdateUserPhoneNumber([FromBody] UserDTO updatedUser)
        {
            await _userService.UpdateUserPhoneNumberAsync(updatedUser);
            return Ok("Номер обновлен");
        }
    }
}
