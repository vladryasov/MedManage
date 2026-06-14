using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность пользователя.
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Имя пользователя (логин).
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Роль пользователя в системе.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Дата и время создания записи о пользователе.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Контактный номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Идентификатор организации, к которой принадлежит пользователь.
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Организация, к которой принадлежит пользователь.
    /// </summary>
    public Organization? Organization { get; set; }

    /// <summary>
    /// Объявления, созданные пользователем.
    /// </summary>
    public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}
