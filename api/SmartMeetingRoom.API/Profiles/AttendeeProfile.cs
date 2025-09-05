using AutoMapper;

using SmartMeetingRoom.API.DTOs.Attendee;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Profiles
{
    public class AttendeeProfile : Profile
    {
        public AttendeeProfile()
        {
            CreateMap<Attendee, AttendeeDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.FkUser))
                .ForMember(dest => dest.AttendeeId, opt => opt.MapFrom(src => src.AttendeeId))
                .ForMember(dest => dest.AttendeeStatus, opt => opt.MapFrom(src => src.AttendeeStatus));

            CreateMap<AttendeeUpdateDto, Attendee>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
