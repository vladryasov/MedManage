namespace MedManage.Domain.Entities;

/// <summary>
/// Сущность организации (медицинское учреждение, поставщик и т.д.).
/// </summary>
public class Organization
{
    /// <summary>
    /// Уникальный идентификатор организации.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Название организации.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Адрес организации.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Контактный номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Контактный email.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Дата и время создания записи об организации.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Пользователи, принадлежащие организации.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Продукты, принадлежащие организации.
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();

    /// <summary>
    /// Объявления, относящиеся к организации.
    /// </summary>
    public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}
