using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;

namespace MedManage.Application.Mappings;

public class AnnouncementMappingProfile : Profile
{
    public AnnouncementMappingProfile()
    {
        CreateMap<Announcement, AnnouncementDTO>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName));

        CreateMap<AnnouncementDTO, Announcement>()
            .ForMember(d => d.User, o => o.Ignore())
            .ForMember(d => d.Organization, o => o.Ignore());
    }
}
