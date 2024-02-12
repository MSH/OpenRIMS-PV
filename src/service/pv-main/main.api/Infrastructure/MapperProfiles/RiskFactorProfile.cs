using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class RiskFactorProfile : Profile
    {
        public RiskFactorProfile()
        {
            CreateMap<RiskFactor, RiskFactorIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"));
            CreateMap<RiskFactor, RiskFactorDetailDto>()
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"));

            CreateMap<RiskFactorOption, RiskFactorOptionDto>();
        }
    }
}
