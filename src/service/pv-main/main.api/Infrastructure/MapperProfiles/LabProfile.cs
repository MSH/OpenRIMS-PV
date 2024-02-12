using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class LabProfile : Profile
    {
        public LabProfile()
        {
            CreateMap<LabResult, LabResultIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.LabResultName, opt => opt.MapFrom(src => src.Description));

            CreateMap<LabTest, LabTestIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.LabTestName, opt => opt.MapFrom(src => src.Description));

            CreateMap<LabTestUnit, LabTestUnitIdentifierDto>()
                .ForMember(dest => dest.LabTestUnitName, opt => opt.MapFrom(src => src.Description));
        }
    }
}
