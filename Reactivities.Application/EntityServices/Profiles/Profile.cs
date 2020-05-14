using System.Collections.Generic;
using Reactivities.Domain.Entities;

namespace Reactivities.Application.EntityServices.Profiles
{
    public class Profile
    {
        public string DisplayName { get; set; }

        public string Username { get; set; }

        public string Image { get; set; }

        public string Biography { get; set; }

        public ICollection<Photo> Photos { get; set; }  
    }
}
