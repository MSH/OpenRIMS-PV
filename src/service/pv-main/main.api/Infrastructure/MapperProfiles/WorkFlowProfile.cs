using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class WorkFlowProfile : Profile
    {
        public WorkFlowProfile()
        {
            CreateMap<WorkFlow, WorkFlowIdentifierDto>()
                .ForMember(dest => dest.WorkFlowName, opt => opt.MapFrom(src => src.Description));
            CreateMap<WorkFlow, WorkFlowDetailDto>()
                .ForMember(dest => dest.WorkFlowName, opt => opt.MapFrom(src => src.Description));
        }
    }
}
