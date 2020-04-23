using Microsoft.EntityFrameworkCore;

namespace Reactivities.Persistence
{
    public class ReactivitiesDbContext : DbContext
    {
        public ReactivitiesDbContext(DbContextOptions options) : base(options) { }
    }
}
