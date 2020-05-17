using System.Linq;
using AutoMapper;
using Reactivities.Domain.Entities;

namespace Reactivities.Application.EntityServices.Activities
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<Activity, ActivityDto>()
                .ForMember(d => d.Attendees, o => o.MapFrom(s => s.UserActivities));

            CreateMap<UserActivity, AttendeeDto>()
                .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.User.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(d => d.Following, o => o.MapFrom<FollowingResolver>());
        }
    }
}
