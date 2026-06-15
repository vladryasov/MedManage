namespace MedManage.Domain.Enums;

/// <summary>
/// Перечисление прав доступа пользователей.
/// </summary>
public enum Permissions
{
    /// <summary>
    /// Обычный пользователь.
    /// </summary>
    User = 0,

    /// <summary>
    /// Специальный пользователь.
    /// </summary>
    SpeciaUser = 1,

    /// <summary>
    /// Администратор.
    /// </summary>
    Admin = 2
}