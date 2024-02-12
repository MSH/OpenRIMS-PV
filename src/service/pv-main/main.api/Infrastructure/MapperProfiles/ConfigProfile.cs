using AutoMapper;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using System;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class ConfigProfile : Profile
    {
        public ConfigProfile()
        {
            CreateMap<Config, ConfigIdentifierDto>()
                .ForMember(dest => dest.ConfigType, opt => opt.MapFrom(src => src.ConfigType));

            CreateMap<Holiday, HolidayIdentifierDto>()
                .ForMember(dest => dest.HolidayDate, opt => opt.MapFrom(src => src.HolidayDate.Date)); ;
        }
    }
}
