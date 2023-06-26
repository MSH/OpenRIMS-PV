using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
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
