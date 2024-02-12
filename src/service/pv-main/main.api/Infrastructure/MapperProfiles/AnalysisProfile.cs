using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class AnalysisProfile : Profile
    {
        public AnalysisProfile()
        {
            CreateMap<ContingencyAnalysisList, AnalyserTermIdentifierDto>()
                .ForMember(dest => dest.TerminologyMeddraId, opt => opt.MapFrom(src => src.TerminologyMeddra_Id));

            CreateMap<TerminologyMedDra, AnalyserTermDetailDto>()
                .ForMember(dest => dest.TerminologyMeddraId, opt => opt.MapFrom(src => src.Id));
            CreateMap<ContingencyAnalysisItem, AnalyserResultDto>()
                .ForMember(dest => dest.Medication, opt => opt.MapFrom(src => src.Drug))
                .ForMember(dest => dest.MedicationId, opt => opt.MapFrom(src => src.Medication_Id))
                .ForPath(dest => dest.ExposedIncidenceRate.Cases, opt => opt.MapFrom(src => src.ExposedCases))
                .ForPath(dest => dest.ExposedIncidenceRate.NonCases, opt => opt.MapFrom(src => src.ExposedNonCases))
                .ForPath(dest => dest.ExposedIncidenceRate.IncidenceRate, opt => opt.MapFrom(src => src.ExposedIncidenceRate))
                .ForPath(dest => dest.ExposedIncidenceRate.Population, opt => opt.MapFrom(src => src.ExposedPopulation))
                .ForPath(dest => dest.NonExposedIncidenceRate.Cases, opt => opt.MapFrom(src => src.NonExposedCases))
                .ForPath(dest => dest.NonExposedIncidenceRate.NonCases, opt => opt.MapFrom(src => src.NonExposedNonCases))
                .ForPath(dest => dest.NonExposedIncidenceRate.IncidenceRate, opt => opt.MapFrom(src => src.NonExposedIncidenceRate))
                .ForPath(dest => dest.NonExposedIncidenceRate.Population, opt => opt.MapFrom(src => src.NonExposedPopulation));

            CreateMap<ContingencyAnalysisPatient, AnalyserPatientDto>()
                .ForMember(dest => dest.Medication, opt => opt.MapFrom(src => src.Drug))
                .ForMember(dest => dest.ExperiencedReaction, opt => opt.MapFrom(src => src.ADR == 1 ? "Yes" : "No"));
        }
    }
}