using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentIdentifierDto>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.AppointmentDate.ToString("yyyy-MM-dd")));
            CreateMap<Appointment, AppointmentDetailDto>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.AppointmentDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.DidNotArrive, opt => opt.MapFrom(src => src.Dna))
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedDetail, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.Cancelled ? "Yes" : "No"));
        }
    }
}