using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;

namespace MedManage.Application.Mappings
{
    /// <summary>
    /// Предоставляет маппинг между сущностью <see cref="Organization"/> и DTO <see cref="OrganizationDTO"/>.
    /// </summary>
    public class OrganizationMappingProfile : Profile
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="OrganizationMappingProfile"/>.
        /// Настраивает маппинг AutoMapper для сущности Organization и её DTO.
        /// </summary>
        public OrganizationMappingProfile()
        {
            CreateMap<Organization, OrganizationDTO>()
                .ReverseMap();
        }
    }
}