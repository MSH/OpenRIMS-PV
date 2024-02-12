using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using System;

namespace OpenRIMS.PV.Main.API.MapperProfiles  
{
    public class CohortGroupProfile : Profile
    {
        public CohortGroupProfile()
        {
            CreateMap<CohortGroup, CohortGroupIdentifierDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("yyyy-MM-dd")));
            CreateMap<CohortGroup, CohortGroupDetailDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.FinishDate, opt => opt.MapFrom(src => src.FinishDate == null ? "" : Convert.ToDateTime(src.FinishDate).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.ConditionName, opt => opt.MapFrom(src => src.Condition.Description))
                .ForMember(dest => dest.EnrolmentCount, opt => opt.MapFrom(src => src.CohortGroupEnrolments.Count));
            CreateMap<CohortGroup, CohortGroupPatientDetailDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Condition.Description))
                .ForMember(dest => dest.ConditionStartDate, opt => opt.Ignore())
                .ForMember(dest => dest.CohortGroupEnrolment, opt => opt.Ignore());

            CreateMap<CohortGroupEnrolment, EnrolmentIdentifierDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.CohortGroupId, opt => opt.MapFrom(src => src.CohortGroup.Id))
                .ForMember(dest => dest.EnroledDate, opt => opt.MapFrom(src => src.EnroledDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.DeenroledDate, opt => opt.MapFrom(src => src.DeenroledDate == null ? "" : Convert.ToDateTime(src.DeenroledDate).ToString("yyyy-MM-dd")));
            CreateMap<CohortGroupEnrolment, EnrolmentDetailDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.CohortGroupId, opt => opt.MapFrom(src => src.CohortGroup.Id))
                .ForMember(dest => dest.EnroledDate, opt => opt.MapFrom(src => src.EnroledDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.DeenroledDate, opt => opt.MapFrom(src => src.DeenroledDate == null ? "" : Convert.ToDateTime(src.DeenroledDate).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.Surname}"))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.Patient.CurrentFacilityName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Patient.Age))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Patient.DateOfBirth == null ? "" : Convert.ToDateTime(src.Patient.DateOfBirth).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.LatestEncounterDate, opt => opt.MapFrom(src => src.Patient.LatestEncounterDate.HasValue ? src.Patient.LatestEncounterDate.Value.ToString("yyyy-MM-dd") : ""));
        }
    }
}