using AutoMapper;

using SmartMeetingRoom.API.Models;
using SmartMeetingRoom.API.DTOs.Role;

namespace SmartMeetingRoom.API.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<ApplicationRole, RoleDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.RoleDescription, opt =>
                       opt.MapFrom(src => string.IsNullOrEmpty(src.RoleDescription) ? "No description provided." : src.RoleDescription));

        CreateMap<RoleCreateDto, ApplicationRole>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName))
            .ForMember(dest => dest.RoleDescription, opt =>
                       opt.MapFrom(src => string.IsNullOrEmpty(src.RoleDescription) ? "No description provided." : src.RoleDescription));

        CreateMap<RoleUpdateDto, ApplicationRole>()
            .ForMember(dest => dest.RoleDescription, opt =>
                opt.MapFrom(src => string.IsNullOrEmpty(src.RoleDescription) ? "No description provided." : src.RoleDescription));
    }
}
