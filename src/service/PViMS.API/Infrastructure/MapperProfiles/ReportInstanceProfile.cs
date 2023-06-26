using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;
using System;
using System.Linq;

namespace PVIMS.API.MapperProfiles
{
    public class ReportInstanceProfile : Profile
    {
        public ReportInstanceProfile()
        {
            CreateMap<ReportInstance, ReportInstanceIdentifierDto>();
            CreateMap<ReportInstance, ReportInstanceDetailDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.CurrentActivity.CurrentStatus.Description))
                .ForMember(dest => dest.QualifiedName, opt => opt.MapFrom(src => src.CurrentActivity.QualifiedName))
                .ForMember(dest => dest.ReportClassification, opt => opt.MapFrom(src => ReportClassification.From(src.ReportClassificationId).Name))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count()))
                .ForMember(dest => dest.ActiveTaskCount, opt => opt.MapFrom(src => src.Tasks.Where(t => t.TaskStatusId != TaskStatus.Cancelled.Id && t.TaskStatusId != TaskStatus.Done.Id && t.TaskStatusId != TaskStatus.Completed.Id).Count()));
            CreateMap<ReportInstance, ReportInstanceExpandedDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.CurrentActivity.CurrentStatus.Description))
                .ForMember(dest => dest.QualifiedName, opt => opt.MapFrom(src => src.CurrentActivity.QualifiedName))
                .ForMember(dest => dest.ReportClassification, opt => opt.MapFrom(src => ReportClassification.From(src.ReportClassificationId).Name))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count()))
                .ForMember(dest => dest.ActiveTaskCount, opt => opt.MapFrom(src => src.Tasks.Where(t => t.TaskStatusId != TaskStatus.Cancelled.Id && t.TaskStatusId != TaskStatus.Done.Id && t.TaskStatusId != TaskStatus.Completed.Id).Count()));

            CreateMap<TerminologyMedDra, TerminologyMedDraDto>();

            CreateMap<ReportInstanceMedication, ReportInstanceMedicationIdentifierDto>()
                .ForMember(dest => dest.ReportInstanceId, opt => opt.MapFrom(src => src.ReportInstance.Id));
            CreateMap<ReportInstanceMedication, ReportInstanceMedicationDetailDto>()
                .ForMember(dest => dest.ReportInstanceId, opt => opt.MapFrom(src => src.ReportInstance.Id));

            CreateMap<ReportInstanceTask, TaskDto>()
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.TaskDetail.Source))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TaskDetail.Description))
                .ForMember(dest => dest.TaskType, opt => opt.MapFrom(src => TaskType.From(src.TaskTypeId).Name))
                .ForMember(dest => dest.TaskStatus, opt => opt.MapFrom(src => TaskStatus.From(src.TaskStatusId).Name))
                .ForMember(dest => dest.TaskAge, opt => opt.MapFrom(src => (DateTime.UtcNow.Date - src.Created.Date).TotalDays))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.OrderByDescending(c => c.Created)));

            CreateMap<ReportInstanceTaskComment, TaskCommentDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.CreatedInitials, opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName.Substring(0, 1)}{src.CreatedBy.LastName.Substring(0, 1)}"))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));

            CreateMap<ActivityExecutionStatusEvent, ActivityExecutionStatusEventDto>()
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.ExecutionStatus.Activity.QualifiedName))
                .ForMember(dest => dest.ExecutionEvent, opt => opt.MapFrom(src => src.ExecutionStatus.Description))
                .ForMember(dest => dest.ExecutedBy, opt => opt.MapFrom(src => src.EventCreatedBy != null ? src.EventCreatedBy.FullName : string.Empty))
                .ForMember(dest => dest.ExecutedDate, opt => opt.MapFrom(src => src.EventDateTime.ToString("yyyy-MM-dd HH:mm tt")))
                .ForMember(dest => dest.ReceiptDate, opt => opt.MapFrom(src => src.ContextDateTime != null ? Convert.ToDateTime(src.ContextDateTime).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.ReceiptCode, opt => opt.MapFrom(src => src.ContextCode));
        }
    }
}