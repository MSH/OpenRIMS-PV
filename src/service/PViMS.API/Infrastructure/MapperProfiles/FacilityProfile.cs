using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class FacilityProfile : Profile
    {
        public FacilityProfile()
        {
            CreateMap<Facility, FacilityIdentifierDto>();
            CreateMap<Facility, FacilityDetailDto>()
                .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.FacilityType.Description))
                .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.TelNumber));

            CreateMap<FacilityType, FacilityTypeIdentifierDto>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Description));
        }
    }
}
