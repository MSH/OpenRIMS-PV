using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class CustomAttributeProfile : Profile
    {
        public CustomAttributeProfile()
        {
            CreateMap<CustomAttributeConfiguration, CustomAttributeIdentifierDto>();
            CreateMap<CustomAttributeConfiguration, CustomAttributeDetailDto>()
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.IsRequired));
        }
    }
}
