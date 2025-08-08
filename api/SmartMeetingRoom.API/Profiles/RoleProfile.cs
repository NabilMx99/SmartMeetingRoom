using AutoMapper;
using SmartMeetingRoom.API.Models;
using SmartMeetingRoom.API.DTOs.Role;

namespace SmartMeetingRoom.API.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.RoleDescription, opt =>
             opt.MapFrom(src => string.IsNullOrEmpty(src.RoleDescription) ? "No description provided." : src.RoleDescription));

        CreateMap<RoleCreateDto, Role>()
            .ForMember(dest => dest.RoleDescription, opt =>
            opt.MapFrom(src => string.IsNullOrEmpty(src.RoleDescription) ? "No description provided." : src.RoleDescription));

        CreateMap<RoleUpdateDto, Role>()
            .ForMember(dest => dest.RoleDescription, opt =>
            opt.MapFrom(src => string.IsNullOrEmpty(src.RoleDescription) ? "No description provided." : src.RoleDescription));
    }
}
