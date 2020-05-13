using AutoMapper;
using Reactivities.Domain.Entities;

namespace Reactivities.Application.EntityServices.Activities
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<Activity, ActivityDto>();

            CreateMap<UserActivity, AttendeeDto>()
                .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName));
        }
    }
}
