using System.Threading.Tasks;

namespace Reactivities.Application.EntityServices.Profiles
{
    public interface IProfileReader
    {
        Task<Profile> ReadProfile(string username);
    }
}
