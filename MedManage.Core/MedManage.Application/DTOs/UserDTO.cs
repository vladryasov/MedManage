using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Application.DTOs;

/// <summary>
/// DTO для пользователя.
/// </summary>
public class UserDTO
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Роль пользователя в системе.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Дата и время создания пользователя.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    public string PhoneNumber { get; set; }
}

/// <summary>
/// Контактный номер