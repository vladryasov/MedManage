using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;

namespace MedManage.Application.Mappings
{
    /// <summary>
    /// Предоставляет маппинг между сущностью <see cref="User"/> и DTO <see cref="UserDTO"/>.
    /// </summary>
    public class UserMappingProfile : Profile
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserMappingProfile"/>.
        /// Настраивает маппинг AutoMapper для сущности User и её DTO.
        /// </summary>
        public UserMappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ReverseMap();
        }
    }
}