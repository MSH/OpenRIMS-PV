using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class OrgUnitProfile : Profile
    {
        public OrgUnitProfile()
        {
            CreateMap<OrgUnit, OrgUnitIdentifierDto>()
                .ForMember(dest => dest.OrgUnitName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.OrgUnitType, opt => opt.MapFrom(src => src.OrgUnitType.Description));
        }
    }
}
