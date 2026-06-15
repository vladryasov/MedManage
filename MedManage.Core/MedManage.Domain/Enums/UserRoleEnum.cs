namespace MedManage.Domain.Enums;

/// <summary>
/// Перечисление ролей пользователей.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Все роли пользователей.
    /// </summary>
    All = 0,

    /// <summary>
    /// Обычный пользователь.
    /// </summary>
    CommonUser = 1,

    /// <summary>
    /// Администратор.
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Специальный пользователь.
    /// </summary>
    SpecialUser = 3
}