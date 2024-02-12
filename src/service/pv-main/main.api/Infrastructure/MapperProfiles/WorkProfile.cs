using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using System.Linq;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class WorkProfile : Profile
    {
        public WorkProfile()
        {
            CreateMap<CareEvent, CareEventIdentifierDto>()
                .ForMember(dest => dest.CareEventName, opt => opt.MapFrom(src => src.Description));

            CreateMap<Dataset, DatasetIdentifierDto>();
            CreateMap<Dataset, DatasetDetailDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.ContextType, opt => opt.MapFrom(src => src.ContextType.Description))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"));
            CreateMap<Dataset, DatasetForSpontaneousDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.ContextType, opt => opt.MapFrom(src => src.ContextType.Description))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DatasetCategories, opt => opt.Ignore());

            CreateMap<DatasetInstance, DatasetInstanceIdentifierDto>();
            CreateMap<DatasetInstance, DatasetInstanceDetailDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));

            CreateMap<DatasetCategory, DatasetCategoryIdentifierDto>();
            CreateMap<DatasetCategory, DatasetCategoryDetailDto>()
                .ForMember(dest => dest.ElementCount, opt => opt.MapFrom(src => src.DatasetCategoryElements.Count))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System ? "Yes" : "No"))
                .ForMember(dest => dest.Acute, opt => opt.MapFrom(src => src.Acute ? "Yes" : "No"))
                .ForMember(dest => dest.Chronic, opt => opt.MapFrom(src => src.Chronic ? "Yes" : "No"));

            CreateMap<DatasetCategoryElement, DatasetCategoryElementIdentifierDto>()
                .ForMember(dest => dest.DatasetElementId, opt => opt.MapFrom(src => src.DatasetElement.Id))
                .ForMember(dest => dest.ElementName, opt => opt.MapFrom(src => src.DatasetElement.ElementName));
            CreateMap<DatasetCategoryElement, DatasetCategoryElementDetailDto>()
                .ForMember(dest => dest.DatasetElementId, opt => opt.MapFrom(src => src.DatasetElement.Id))
                .ForMember(dest => dest.ElementName, opt => opt.MapFrom(src => src.DatasetElement.ElementName))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System ? "Yes" : "No"))
                .ForMember(dest => dest.Acute, opt => opt.MapFrom(src => src.Acute ? "Yes" : "No"))
                .ForMember(dest => dest.Chronic, opt => opt.MapFrom(src => src.Chronic ? "Yes" : "No"));

            CreateMap<DatasetElement, DatasetElementIdentifierDto>()
                .ForMember(dest => dest.FieldTypeName, opt => opt.MapFrom(src => src.Field.FieldType.Description));
            CreateMap<DatasetElement, DatasetElementDetailDto>()
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System ? "Yes" : "No"))
                .ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Field.Mandatory ? "Yes" : "No"))
                .ForMember(dest => dest.MaxLength, opt => opt.MapFrom(src => src.Field.MaxLength))
                .ForMember(dest => dest.RegEx, opt => opt.MapFrom(src => src.Field.RegEx))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Field.Decimals))
                .ForMember(dest => dest.MaxSize, opt => opt.MapFrom(src => src.Field.MaxSize))
                .ForMember(dest => dest.MinSize, opt => opt.MapFrom(src => src.Field.MinSize))
                .ForMember(dest => dest.Calculation, opt => opt.MapFrom(src => src.Field.Calculation))
                .ForMember(dest => dest.Anonymise, opt => opt.MapFrom(src => src.Field.Anonymise ? "Yes" : "No"))
                .ForMember(dest => dest.FieldTypeName, opt => opt.MapFrom(src => src.Field.FieldType.Description));
            CreateMap<DatasetElement, DatasetElementExpandedDto>()
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System ? "Yes" : "No"))
                .ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Field.Mandatory ? "Yes" : "No"))
                .ForMember(dest => dest.MaxLength, opt => opt.MapFrom(src => src.Field.MaxLength))
                .ForMember(dest => dest.RegEx, opt => opt.MapFrom(src => src.Field.RegEx))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Field.Decimals))
                .ForMember(dest => dest.MaxSize, opt => opt.MapFrom(src => src.Field.MaxSize))
                .ForMember(dest => dest.MinSize, opt => opt.MapFrom(src => src.Field.MinSize))
                .ForMember(dest => dest.Calculation, opt => opt.MapFrom(src => src.Field.Calculation))
                .ForMember(dest => dest.Anonymise, opt => opt.MapFrom(src => src.Field.Anonymise ? "Yes" : "No"))
                .ForMember(dest => dest.FieldTypeName, opt => opt.MapFrom(src => src.Field.FieldType.Description));

            CreateMap<DatasetElementSub, DatasetElementSubDto>()
                .ForMember(dest => dest.FieldTypeName, opt => opt.MapFrom(src => src.Field.FieldType.Description));

            CreateMap<EncounterType, EncounterTypeIdentifierDto>()
                .ForMember(dest => dest.EncounterTypeName, opt => opt.MapFrom(src => src.Description));
            CreateMap<EncounterType, EncounterTypeDetailDto>()
                .ForMember(dest => dest.EncounterTypeName, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.WorkPlanName, opt => opt.MapFrom(src => src.EncounterTypeWorkPlans.First().WorkPlan.Description));

            CreateMap<WorkPlan, WorkPlanIdentifierDto>()
                .ForMember(dest => dest.WorkPlanName, opt => opt.MapFrom(src => src.Description));
            CreateMap<WorkPlan, WorkPlanDetailDto>()
                .ForMember(dest => dest.WorkPlanName, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DatasetName, opt => opt.MapFrom(src => src.Dataset.DatasetName));

        }
    }
}
