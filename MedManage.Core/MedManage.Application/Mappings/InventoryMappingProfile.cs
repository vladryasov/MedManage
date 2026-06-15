using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;

namespace MedManage.Application.Mappings
{
    /// <summary>
    /// Предоставляет маппинг между сущностью <see cref="Inventory"/> и DTO <see cref="InventoryDTO"/>.
    /// </summary>
    public class InventoryMappingProfile : Profile
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="InventoryMappingProfile"/>.
        /// Настраивает маппинг AutoMapper для сущности Inventory и её DTO.
        /// </summary>
        public InventoryMappingProfile()
        {
            CreateMap<Inventory, InventoryDTO>()
                .ReverseMap();
        }
    }
}