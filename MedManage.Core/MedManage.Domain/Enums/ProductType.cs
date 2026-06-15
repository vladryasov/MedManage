namespace MedManage.Domain.Enums;

/// <summary>
/// Перечисление типов продуктов.
/// </summary>
public enum ProductType
{
    /// <summary>
    /// Все типы продуктов.
    /// </summary>
    All = 0,

    /// <summary>
    /// Таблетки.
    /// </summary>
    Tablet = 1,

    /// <summary>
    /// Медицинское оборудование.
    /// </summary>
    MedicineEquipment = 2,

    /// <summary>
    /// Шприцы.
    /// </summary>
    Syringe = 3,

    /// <summary>
    /// Аппараты искусственной вентиляции легких.
    /// </summary>
    Ventilator = 4,

    /// <summary>
    /// Хирургические инструменты.
    /// </summary>
    SurgicalInstrument = 5,

    /// <summary>
    /// Расходные материалы.
    /// </summary>
    Consumable = 6
}