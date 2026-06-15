using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Application.DTOs;

/// <summary>
/// DTO для продукта.
/// </summary>
public class ProductDTO
{
    /// <summary>
    /// Уникальный идентификатор продукта.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название продукта.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Тип продукта.
    /// </summary>
    public ProductType Type { get; set; }

    /// <summary>
    /// Цена продукта.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Дата истечения срока годности продукта.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Инвентарная информация, связанная с продуктом.
    /// </summary>
    public Inventory Inventory { get; set; }
}