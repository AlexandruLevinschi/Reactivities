namespace Reactivities.Domain.Entities
{
    public class UserFollowing
    {
        public string ObserverId { get; set; }

        public virtual User Observer { get; set; }

        public string TargetId { get; set; }

        public virtual User Target { get; set; }
    }
}
