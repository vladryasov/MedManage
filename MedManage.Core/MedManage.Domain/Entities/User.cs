using MedManage.Domain.Enums;

namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность пользователя.
/// </summary>
public class User
{
    private User()
    {
    }

    public User(
        string userName,
        string fullName,
        string email,
        UserRole role,
        string phoneNumber,
        string? passwordHash = null,
        Guid? organizationId = null)
    {
        UserId = Guid.NewGuid();
        UserName = userName;
        FullName = fullName;
        Email = email;
        Role = role;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        OrganizationId = organizationId;
        CreatedAt = DateTime.UtcNow;
    }

    public User(
    Guid userId,
    string userName,
    string fullName,
    string email,
    UserRole role,
    string phoneNumber,
    string? passwordHash,
    DateTime createdAt,
    Guid? organizationId = null)
    {
        UserId = userId;
        UserName = userName;
        FullName = fullName;
        Email = email;
        Role = role;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        OrganizationId = organizationId;
    }

    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Имя пользователя (логин).
    /// </summary>
    public string UserName { get; private set; } = null!;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Роль пользователя в системе.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Хеш пароля пользователя.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Дата и время создания записи о пользователе.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

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
    public ICollection<Announcement> Announcements { get; private set; } = new List<Announcement>();
    public ICollection<PurchaseRequest> PurchaseRequestsAsBuyer { get; private set; } = new List<PurchaseRequest>();
    public ICollection<PurchaseRequest> PurchaseRequestsAsSeller { get; private set; } = new List<PurchaseRequest>();
}
