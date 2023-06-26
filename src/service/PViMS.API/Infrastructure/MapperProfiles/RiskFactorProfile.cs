using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
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
