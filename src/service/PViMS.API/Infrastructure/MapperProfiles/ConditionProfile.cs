using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using System;

namespace PVIMS.API.MapperProfiles
{
    public class ConditionProfile : Profile
    {
        public ConditionProfile()
        {
            CreateMap<Condition, ConditionIdentifierDto>()
                .ForMember(dest => dest.ConditionName, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"));
            CreateMap<Condition, ConditionDetailDto>()
                .ForMember(dest => dest.ConditionName, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.Chronic, opt => opt.MapFrom(src => src.Chronic ? "Yes" : "No"));

            CreateMap<ConditionLabTest, ConditionLabTestDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LabTestId, opt => opt.MapFrom(src => src.LabTest.Id))
                .ForMember(dest => dest.LabTestName, opt => opt.MapFrom(src => src.LabTest.Description));

            CreateMap<ConditionMedication, ConditionMedicationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Concept.Id))
                .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src => $"{src.Concept.ConceptName} ({src.Concept.MedicationForm.Description})"));

            CreateMap<ConditionMedDra, ConditionMeddraDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TerminologyMedDraId, opt => opt.MapFrom(src => src.TerminologyMedDra.Id))
                .ForMember(dest => dest.MedDraTerm, opt => opt.MapFrom(src => src.TerminologyMedDra.MedDraTerm));

            CreateMap<Outcome, OutcomeIdentifierDto>()
                .ForMember(dest => dest.OutcomeName, opt => opt.MapFrom(src => src.Description));
            CreateMap<TreatmentOutcome, TreatmentOutcomeIdentifierDto>()
                .ForMember(dest => dest.TreatmentOutcomeName, opt => opt.MapFrom(src => src.Description));
        }
    }
}
