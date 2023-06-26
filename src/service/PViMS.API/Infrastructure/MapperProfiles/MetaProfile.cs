using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class MetaProfile : Profile
    {
        public MetaProfile()
        {
            CreateMap<MetaColumn, MetaColumnIdentifierDto>()
                .ForMember(dest => dest.MetaColumnGuid, opt => opt.MapFrom(src => src.MetaColumnGuid));
            CreateMap<MetaColumn, MetaColumnDetailDto>()
                .ForMember(dest => dest.MetaColumnGuid, opt => opt.MapFrom(src => src.MetaColumnGuid))
                .ForMember(dest => dest.ColumnType, opt => opt.MapFrom(src => src.ColumnType.Description))
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Table.TableName));
            CreateMap<MetaColumn, MetaColumnExpandedDto>()
                .ForMember(dest => dest.MetaColumnGuid, opt => opt.MapFrom(src => src.MetaColumnGuid))
                .ForMember(dest => dest.ColumnType, opt => opt.MapFrom(src => src.ColumnType.Description))
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Table.TableName));

            CreateMap<MetaDependency, MetaDependencyIdentifierDto>()
                .ForMember(dest => dest.MetaDependencyGuid, opt => opt.MapFrom(src => src.MetaDependencyGuid));
            CreateMap<MetaDependency, MetaDependencyDetailDto>()
                .ForMember(dest => dest.MetaDependencyGuid, opt => opt.MapFrom(src => src.MetaDependencyGuid))
                .ForMember(dest => dest.ParentTableName, opt => opt.MapFrom(src => src.ParentTable.TableName))
                .ForMember(dest => dest.ReferenceTableName, opt => opt.MapFrom(src => src.ReferenceTable.TableName));

            CreateMap<MetaForm, MetaFormIdentifierDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.MetaFormGuid));
            CreateMap<MetaForm, MetaFormDetailDto>()
                .ForMember(dest => dest.MetaFormGuid, opt => opt.MapFrom(src => src.MetaFormGuid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"));

            CreateMap<MetaTable, MetaTableIdentifierDto>()
                .ForMember(dest => dest.MetaTableGuid, opt => opt.MapFrom(src => src.MetaTableGuid));
            CreateMap<MetaTable, MetaTableDetailDto>()
                .ForMember(dest => dest.MetaTableGuid, opt => opt.MapFrom(src => src.MetaTableGuid))
                .ForMember(dest => dest.TableType, opt => opt.MapFrom(src => src.TableType.Description));
            CreateMap<MetaTable, MetaTableExpandedDto>()
                .ForMember(dest => dest.MetaTableGuid, opt => opt.MapFrom(src => src.MetaTableGuid))
                .ForMember(dest => dest.TableType, opt => opt.MapFrom(src => src.TableType.Description));
        }
    }
}