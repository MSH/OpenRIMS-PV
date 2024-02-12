using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.NotificationAggregate;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.CreatedDetail, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.NotificationType, opt => opt.MapFrom(src => NotificationType.From(src.NotificationTypeId).Name))
                .ForMember(dest => dest.NotificationClassification, opt => opt.MapFrom(src => NotificationClassification.From(src.NotificationClassificationId).Name));
        }
    }
}