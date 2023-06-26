using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ContactAggregate;

namespace PVIMS.API.MapperProfiles
{
    public class ContactDetailProfile : Profile
    {
        public ContactDetailProfile()
        {
            CreateMap<SiteContactDetail, ContactIdentifierDto>()
                .ForMember(dest => dest.ContactLastName, opt => opt.MapFrom(src => src.ContactSurname));
            CreateMap<SiteContactDetail, ContactDetailDto>()
                .ForMember(dest => dest.ContactLastName, opt => opt.MapFrom(src => src.ContactSurname));
        }
    }
}
