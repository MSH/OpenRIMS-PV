using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class MetaReportProfile : Profile
    {
        public MetaReportProfile()
        {
            CreateMap<MetaReport, MetaReportIdentifierDto>()
                .ForMember(dest => dest.MetaReportGuid, opt => opt.MapFrom(src => src.MetaReportGuid));
            CreateMap<MetaReport, MetaReportDetailDto>()
                .ForMember(dest => dest.MetaReportGuid, opt => opt.MapFrom(src => src.MetaReportGuid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.ReportStatus, opt => opt.MapFrom(src => src.ReportStatus.ToString()));
            CreateMap<MetaReport, MetaReportExpandedDto>()
                .ForMember(dest => dest.MetaReportGuid, opt => opt.MapFrom(src => src.MetaReportGuid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.ReportStatus, opt => opt.MapFrom(src => src.ReportStatus.ToString()));
        }
    }
}