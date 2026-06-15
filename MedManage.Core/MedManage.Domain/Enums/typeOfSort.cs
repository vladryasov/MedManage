namespace MedManage.Domain.Enums;

/// <summary>
/// Перечисление типов сортировки.
/// </summary>
public enum TypeOfSort
{
    /// <summary>
    /// Без сортировки.
    /// </summary>
    All = 0,

    /// <summary>
    /// Сортировка по дате.
    /// </summary>
    ByDate = 1,

    /// <summary>
    /// Сортировка по категории.
    /// </summary>
    ByCategory = 2
}