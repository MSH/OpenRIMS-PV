using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
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
