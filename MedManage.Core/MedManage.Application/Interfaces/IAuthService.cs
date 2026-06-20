using MedManage.Application.DTOs;

namespace MedManage.Application.Interfaces;

public interface IAuthService
{
    Task<UserDTO> LoginAsync(string token);
}
