using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ContactAggregate;

namespace OpenRIMS.PV.Main.API.MapperProfiles
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
