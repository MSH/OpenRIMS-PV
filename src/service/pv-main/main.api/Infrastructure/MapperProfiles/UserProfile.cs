using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using System;

namespace OpenRIMS.PV.Main.API.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AuditLog, AuditLogIdentifierDto>();
            CreateMap<AuditLog, AuditLogDetailDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.HasLog, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Log)));

            CreateMap<IdentityRole<Guid>, RoleIdentifierDto>();

            CreateMap<User, UserIdentifierDto>();
            CreateMap<User, UserDetailDto>()
                .ForMember(dest => dest.AllowDatasetDownload, opt => opt.MapFrom(src => src.AllowDatasetDownload ? "Yes" : "No"))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "Yes" : "No"));

            CreateMap<UserFacility, UserFacilityDto>()
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.Facility.FacilityName))
                .ForMember(dest => dest.OrgUnitName, opt => opt.MapFrom(src => src.Facility.OrgUnit.Name));
        }
    }
}