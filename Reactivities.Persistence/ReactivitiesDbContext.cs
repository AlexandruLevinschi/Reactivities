using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reactivities.Domain.Entities;

namespace Reactivities.Persistence
{
    public class ReactivitiesDbContext : IdentityDbContext<User>
    {
        public ReactivitiesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserActivity>(x => x.HasKey(ua => new {ua.UserId, ua.ActivityId}));

            builder.Entity<UserActivity>()
                .HasOne(u => u.User)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(u => u.UserId);

            builder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(a => a.ActivityId);
        }
    }
}
