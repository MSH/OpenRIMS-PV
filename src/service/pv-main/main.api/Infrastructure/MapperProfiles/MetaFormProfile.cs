using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class MetaFormProfile : Profile
    {
        public MetaFormProfile()
        {
            CreateMap<MetaForm, MetaFormIdentifierDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.MetaFormGuid));
            CreateMap<MetaForm, MetaFormDetailDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.MetaFormGuid))
                .ForMember(dest => dest.CohortGroup, opt => opt.MapFrom(src => src.CohortGroup.CohortName))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"));
            CreateMap<MetaForm, MetaFormExpandedDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.MetaFormGuid))
                .ForMember(dest => dest.CohortGroup, opt => opt.MapFrom(src => src.CohortGroup.CohortName))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"));

            CreateMap<MetaFormCategory, MetaFormCategoryDto>()
                .ForMember(dest => dest.MetaTableName, opt => opt.MapFrom(src => src.MetaTable.TableName));

            CreateMap<MetaFormCategoryAttribute, MetaFormCategoryAttributeDto>()
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.CustomAttributeConfiguration.AttributeKey));
        }
    }
}