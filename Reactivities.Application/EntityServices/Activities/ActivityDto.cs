using System;
using System.Collections.Generic;
using Reactivities.Application.EntityServices.Comments;

namespace Reactivities.Application.EntityServices.Activities
{
    public class ActivityDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }

        public ICollection<AttendeeDto> Attendees { get; set; }

        public ICollection<CommentDto> Comments { get; set; }
    }
}
