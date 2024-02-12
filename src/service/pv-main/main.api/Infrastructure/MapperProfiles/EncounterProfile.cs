using AutoMapper;
using OpenRIMS.PV.Main.API.Application.Models.Encounter;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Models;
using System.Linq;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class EncounterProfile : Profile
    {
        public EncounterProfile()
        {
            CreateMap<Encounter, EncounterIdentifierDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.EncounterDate, opt => opt.MapFrom(src => src.EncounterDate.ToString("yyyy-MM-dd")));
            CreateMap<Encounter, EncounterDetailDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.EncounterDate, opt => opt.MapFrom(src => src.EncounterDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EncounterType, opt => opt.MapFrom(src => src.EncounterType.Description))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated));
            CreateMap<Encounter, EncounterExpandedDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.EncounterDate, opt => opt.MapFrom(src => src.EncounterDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EncounterType, opt => opt.MapFrom(src => src.EncounterType.Description))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.PatientConditions, opt => opt.MapFrom(src => src.Patient.PatientConditions.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientClinicalEvents, opt => opt.MapFrom(src => src.Patient.PatientClinicalEvents.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientMedications, opt => opt.MapFrom(src => src.Patient.PatientMedications.Where(a => a.Archived == false)))
                .ForMember(dest => dest.PatientLabTests, opt => opt.MapFrom(src => src.Patient.PatientLabTests.Where(a => a.Archived == false)));

            CreateMap<SearchEncounterDto, EncounterIdentifierDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EncounterId));
            CreateMap<SearchEncounterDto, EncounterDetailDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EncounterId));

            CreateMap<Priority, PriorityIdentifierDto>()
                .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Description));

            CreateMap<EncounterForCreationDto, EncounterDetail>();
        }
    }
}