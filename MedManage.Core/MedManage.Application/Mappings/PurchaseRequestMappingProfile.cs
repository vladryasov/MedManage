using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Domain.Entities;

namespace MedManage.Application.Mappings;

public class PurchaseRequestMappingProfile : Profile
{
    public PurchaseRequestMappingProfile()
    {
        CreateMap<PurchaseRequest, PurchaseRequestDTO>()
            .ForMember(d => d.AnnouncementTitle, o => o.MapFrom(s => s.Announcement != null ? s.Announcement.Title : null))
            .ForMember(d => d.BuyerUserName, o => o.MapFrom(s => s.BuyerUser.UserName))
            .ForMember(d => d.SellerUserName, o => o.MapFrom(s => s.SellerUser.UserName));
    }
}
