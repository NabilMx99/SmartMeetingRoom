using AutoMapper;
using SmartMeetingRoom.API.DTOs.Feature;
using SmartMeetingRoom.API.DTOs.Room;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Profiles;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.RoomFeatures.Select(rf => rf.FkFeature)));

        CreateMap<RoomCreateDto, Room>();

        CreateMap<RoomUpdateDto, Room>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Feature, FeatureDto>();
    }
}
