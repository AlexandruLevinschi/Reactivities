using Microsoft.EntityFrameworkCore;
using Reactivities.Domain.Entities;

namespace Reactivities.Persistence
{
    public class ReactivitiesDbContext : DbContext
    {
        public ReactivitiesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Activity> Activities { get; set; }
    }
}
