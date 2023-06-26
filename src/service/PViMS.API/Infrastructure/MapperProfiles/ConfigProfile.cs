using AutoMapper;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using System;

namespace PVIMS.API.MapperProfiles
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
