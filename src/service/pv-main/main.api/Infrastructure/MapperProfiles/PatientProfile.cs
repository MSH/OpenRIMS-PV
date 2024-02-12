using AutoMapper;
using OpenRIMS.PV.Main.API.Application.Models.Patient;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Entities.Keyless;
using OpenRIMS.PV.Main.Core.Models;
using System;
using System.Linq;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientIdentifierDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.CurrentFacilityName))
                .ForMember(dest => dest.OrganisationUnit, opt => opt.MapFrom(src => src.CurrentFacilityOrganisationUnit));
            CreateMap<Patient, PatientDetailDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.CurrentFacilityName))
                .ForMember(dest => dest.OrganisationUnit, opt => opt.MapFrom(src => src.CurrentFacilityOrganisationUnit))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth == null ? "" : Convert.ToDateTime(src.DateOfBirth).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
            CreateMap<Patient, PatientExpandedDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.CurrentFacilityName))
                .ForMember(dest => dest.OrganisationUnit, opt => opt.MapFrom(src => src.CurrentFacilityOrganisationUnit))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth == null ? "" : Convert.ToDateTime(src.DateOfBirth).ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments.Where(a => a.Archived == false)))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments.Where(a => a.Archived == false)))
                .ForMember(dest => dest.Encounters, opt => opt.MapFrom(src => src.Encounters.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientConditions, opt => opt.MapFrom(src => src.PatientConditions.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientClinicalEvents, opt => opt.MapFrom(src => src.PatientClinicalEvents.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientMedications, opt => opt.MapFrom(src => src.PatientMedications.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientLabTests, opt => opt.MapFrom(src => src.PatientLabTests.Where(a => a.Archived == false)));

            CreateMap<PatientForCreationDto, PatientDetailForCreation>()
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName));
            CreateMap<PatientForUpdateDto, PatientDetailForCreation>()
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName));
            CreateMap<PatientForCreationDto, ConditionDetail>()
                .ForMember(dest => dest.OnsetDate, opt => opt.MapFrom(src => src.StartDate));

            CreateMap<PatientSearchDto, PatientIdentifierDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.FacilityName));
            CreateMap<PatientSearchDto, PatientDetailDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.FacilityName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));
            CreateMap<PatientSearchDto, PatientListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.Surname}" ));

            CreateMap<PatientClinicalEvent, PatientClinicalEventIdentifierDto>();
            CreateMap<PatientClinicalEvent, PatientClinicalEventDetailDto>()
                .ForMember(dest => dest.MedDraTerm, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.MedDraTerm))
                .ForMember(dest => dest.SourceTerminologyMedDraId, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.Id))
                .ForMember(dest => dest.OnsetDate, opt => opt.MapFrom(src => src.OnsetDate.HasValue ? Convert.ToDateTime(src.OnsetDate).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.ResolutionDate, opt => opt.MapFrom(src => src.ResolutionDate.HasValue ? Convert.ToDateTime(src.ResolutionDate).ToString("yyyy-MM-dd") : ""));
            CreateMap<PatientClinicalEvent, PatientClinicalEventExpandedDto>()
                .ForMember(dest => dest.MedDraTerm, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.MedDraTerm))
                .ForMember(dest => dest.SourceTerminologyMedDraId, opt => opt.MapFrom(src => src.SourceTerminologyMedDra.Id))
                .ForMember(dest => dest.OnsetDate, opt => opt.MapFrom(src => src.OnsetDate.HasValue ? Convert.ToDateTime(src.OnsetDate).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.ResolutionDate, opt => opt.MapFrom(src => src.ResolutionDate.HasValue ? Convert.ToDateTime(src.ResolutionDate).ToString("yyyy-MM-dd") : ""));

            CreateMap<PatientCondition, PatientConditionIdentifierDto>();
            CreateMap<PatientCondition, PatientConditionDetailDto>()
                .ForMember(dest => dest.SourceDescription, opt => opt.MapFrom(src => src.ConditionSource))
                .ForMember(dest => dest.MedDraTerm, opt => opt.MapFrom(src => src.TerminologyMedDra.MedDraTerm))
                .ForMember(dest => dest.SourceTerminologyMedDraId, opt => opt.MapFrom(src => src.TerminologyMedDra.Id))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.OnsetDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Outcome, opt => opt.MapFrom(src => src.Outcome.Description))
                .ForMember(dest => dest.OutcomeDate, opt => opt.MapFrom(src => src.OutcomeDate.HasValue ? Convert.ToDateTime(src.OutcomeDate).ToString("yyyy-MM-dd") : ""))
                .ForMember(dest => dest.TreatmentOutcome, opt => opt.MapFrom(src => src.TreatmentOutcome.Description));

            CreateMap<PatientLabTest, PatientLabTestIdentifierDto>();
            CreateMap<PatientLabTest, PatientLabTestDetailDto>()
                .ForMember(dest => dest.TestDate, opt => opt.MapFrom(src => src.TestDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.LabTest, opt => opt.MapFrom(src => src.LabTest.Description))
                .ForMember(dest => dest.TestResultCoded, opt => opt.MapFrom(src => src.TestResult))
                .ForMember(dest => dest.TestResultValue, opt => opt.MapFrom(src => src.LabValue))
                .ForMember(dest => dest.TestUnit, opt => opt.MapFrom(src => src.TestUnit.Description));

            CreateMap<PatientMedication, PatientMedicationIdentifierDto>();
            CreateMap<PatientMedication, PatientMedicationDetailDto>()
                .ForMember(dest => dest.SourceDescription, opt => opt.MapFrom(src => src.MedicationSource))
                .ForMember(dest => dest.ConceptId, opt => opt.MapFrom(src => src.Concept.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.Medication, opt => opt.MapFrom(src => src.Product != null ? $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description}) ({src.Product.ProductName})" : $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description})"))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString("yyyy-MM-dd") : ""));

            CreateMap<AdverseEventList, AdverseEventReportDto>()
                .ForMember(dest => dest.AdverseEvent, opt => opt.MapFrom(src => src.Description));
            CreateMap<DrugList, PatientMedicationReportDto>();

            CreateMap<PatientStatusHistory, PatientStatusDto>()
                .ForMember(dest => dest.PatientStatus, opt => opt.MapFrom(src => src.PatientStatus.Description))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
        }
    }
}