using System.Threading.Tasks;
using Reactivities.Application.EntityServices.Users;

namespace Reactivities.Application.Interfaces
{
    public interface IFacebookAccessor
    {
        Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}
