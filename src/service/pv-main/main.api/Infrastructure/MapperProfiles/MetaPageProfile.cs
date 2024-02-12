using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using System.Linq;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class MetaPageProfile : Profile
    {
        public MetaPageProfile()
        {
            CreateMap<MetaPage, MetaPageIdentifierDto>()
                .ForMember(dest => dest.MetaPageGuid, opt => opt.MapFrom(src => src.MetaPageGuid));
            CreateMap<MetaPage, MetaPageDetailDto>()
                .ForMember(dest => dest.MetaPageGuid, opt => opt.MapFrom(src => src.MetaPageGuid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.IsVisible ? "Yes" : "No"));
            CreateMap<MetaPage, MetaPageExpandedDto>()
                .ForMember(dest => dest.MetaPageGuid, opt => opt.MapFrom(src => src.MetaPageGuid))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.IsSystem ? "Yes" : "No"))
                .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.IsVisible ? "Yes" : "No"))
                .ForMember(dest => dest.Widgets, opt => opt.MapFrom(src => src.Widgets.Where(w => w.WidgetStatus == Core.ValueTypes.MetaWidgetStatus.Published)))
                .ForMember(dest => dest.UnpublishedWidgets, opt => opt.MapFrom(src => src.Widgets.Where(w => w.WidgetStatus == Core.ValueTypes.MetaWidgetStatus.Unpublished)));

            CreateMap<MetaWidget, MetaWidgetIdentifierDto>()
                .ForMember(dest => dest.MetaWidgetGuid, opt => opt.MapFrom(src => src.MetaWidgetGuid));
            CreateMap<MetaWidget, MetaWidgetDetailDto>()
                .ForMember(dest => dest.MetaWidgetGuid, opt => opt.MapFrom(src => src.MetaWidgetGuid))
                .ForMember(dest => dest.WidgetType, opt => opt.MapFrom(src => src.WidgetType.Description));
        }
    }
}