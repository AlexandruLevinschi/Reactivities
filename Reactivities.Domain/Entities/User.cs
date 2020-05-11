using Microsoft.AspNetCore.Identity;

namespace Reactivities.Domain.Entities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
