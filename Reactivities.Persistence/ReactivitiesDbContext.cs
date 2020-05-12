using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reactivities.Domain.Entities;

namespace Reactivities.Persistence
{
    public class ReactivitiesDbContext : IdentityDbContext<User>
    {
        public ReactivitiesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Activity> Activities { get; set; }
    }
}
