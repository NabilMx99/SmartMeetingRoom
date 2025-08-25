using AutoMapper;
using SmartMeetingRoom.API.DTOs.Feature;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Profiles;

public class FeatureProfile : Profile
{
    public FeatureProfile()
    {
        CreateMap<Feature, FeatureDto>();
        
        CreateMap<FeatureCreateDto, Feature>();

        CreateMap<FeatureUpdateDto, Feature>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
