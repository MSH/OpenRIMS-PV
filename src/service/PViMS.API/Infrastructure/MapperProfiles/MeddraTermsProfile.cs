using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class MeddraTermsProfile : Profile
    {
        public MeddraTermsProfile()
        {
            CreateMap<TerminologyMedDra, MeddraTermIdentifierDto>();
            CreateMap<TerminologyMedDra, MeddraTermDetailDto>()
                .ForMember(dest => dest.ParentMedDraTerm, opt => opt.MapFrom(src => src.Parent.MedDraTerm));
        }
    }
}
