using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
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
