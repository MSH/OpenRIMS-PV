using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ConceptAggregate;
using PVIMS.Core.Entities;

namespace PVIMS.API.MapperProfiles
{
    public class ConceptProfile : Profile
    {
        public ConceptProfile()
        {
            CreateMap<Concept, ConceptIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.ConceptName}; {src.Strength} ({src.MedicationForm.Description})"));
            CreateMap<Concept, ConceptDetailDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.ConceptName}; {src.Strength} ({src.MedicationForm.Description})"))
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.MedicationForm.Description));

            CreateMap<Product, ProductIdentifierDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Concept.ConceptName}; {src.Concept.Strength} ({src.Concept.MedicationForm.Description}) ({src.ProductName})" ));
            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"))
                .ForMember(dest => dest.ConceptDisplayName, opt => opt.MapFrom(src => $"{src.Concept.ConceptName}; {src.Concept.Strength} ({src.Concept.MedicationForm.Description})"))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Concept.ConceptName}; {src.Concept.Strength} ({src.Concept.MedicationForm.Description}) ({src.ProductName})"))
                .ForMember(dest => dest.ConceptName, opt => opt.MapFrom(src => src.Concept.ConceptName))
                .ForMember(dest => dest.Strength, opt => opt.MapFrom(src => src.Concept.Strength))
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Concept.MedicationForm.Description));

            CreateMap<MedicationForm, MedicationFormIdentifierDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Description));
        }
    }
}
