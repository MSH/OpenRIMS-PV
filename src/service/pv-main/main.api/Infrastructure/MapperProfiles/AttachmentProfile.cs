using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<Attachment, AttachmentIdentifierDto>();
            CreateMap<Attachment, AttachmentDetailDto>()
                .ForMember(dest => dest.AttachmentType, opt => opt.MapFrom(src => src.AttachmentType.Description))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
        }
    }
}