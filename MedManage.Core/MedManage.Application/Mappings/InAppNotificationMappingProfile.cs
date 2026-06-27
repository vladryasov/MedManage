using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;

namespace MedManage.Application.Mappings;

public class InAppNotificationMappingProfile : Profile
{
    public InAppNotificationMappingProfile()
    {
        CreateMap<InAppNotification, InAppNotificationDTO>()
            .ForMember(d => d.SenderUserName, o => o.MapFrom(s => s.SenderUser != null ? s.SenderUser.UserName : null));
    }
}
