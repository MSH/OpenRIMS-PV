using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
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
