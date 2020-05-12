using Reactivities.Domain.Entities;

namespace Reactivities.Application.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(User user);
    }
}
