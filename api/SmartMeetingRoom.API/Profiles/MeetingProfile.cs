using AutoMapper;

using SmartMeetingRoom.API.DTOs.Meeting;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Profiles
{
    public class MeetingProfile : Profile
    {
        public MeetingProfile()
        {
            CreateMap<Meeting, MeetingDto>()
                .ForMember(dest => dest.MeetingId, opt => opt.MapFrom(src => src.MeetingId))
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.FkUser))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.FkRoom))
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees))
                .ForMember(dest => dest.MeetingStartTime, opt => opt.MapFrom(src => src.MeetingStartTime))
                .ForMember(dest => dest.MeetingEndTime, opt => opt.MapFrom(src => src.MeetingEndTime))
                .ForMember(dest => dest.MeetingTitle, opt => opt.MapFrom(src => src.MeetingTitle))
                .ForMember(dest => dest.MeetingAgenda, opt => opt.MapFrom(src => src.MeetingAgenda))
                .ForMember(dest => dest.MeetingStatus, opt => opt.MapFrom(src => src.MeetingStatus));

            CreateMap<MeetingCreateDto, Meeting>()
                .ForMember(dest => dest.MeetingId, opt => opt.Ignore())
                .ForMember(dest => dest.MeetingStatus, opt => opt.MapFrom(src => "Scheduled"))
                .ForMember(dest => dest.Attendees, opt => opt.Ignore())
                .ForMember(dest => dest.FkUser, opt => opt.Ignore())
                .ForMember(dest => dest.FkRoom, opt => opt.Ignore())
                .ForMember(dest => dest.MeetingMinutes, opt => opt.Ignore());

            CreateMap<MeetingUpdateDto, Meeting>()
                .ForMember(dest => dest.MeetingStartTime, opt => opt.Condition(src => src.MeetingStartTime.HasValue))
                .ForMember(dest => dest.MeetingEndTime, opt => opt.Condition(src => src.MeetingEndTime.HasValue))
                .ForMember(dest => dest.MeetingTitle, opt => opt.Condition(src => !string.IsNullOrEmpty(src.MeetingTitle)))
                .ForMember(dest => dest.MeetingAgenda, opt => opt.Condition(src => !string.IsNullOrEmpty(src.MeetingAgenda)))
                .ForMember(dest => dest.MeetingStatus, opt => opt.Condition(src => !string.IsNullOrEmpty(src.MeetingStatus)))
                .ForMember(dest => dest.Attendees, opt => opt.Ignore())
                .ForMember(dest => dest.FkUser, opt => opt.Ignore())
                .ForMember(dest => dest.FkRoom, opt => opt.Ignore())
                .ForMember(dest => dest.MeetingMinutes, opt => opt.Ignore());
        }
    }
}


