using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Reactivities.Domain.Entities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }

        public string Biography { get; set; }

        public virtual ICollection<UserActivity> UserActivities { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}
