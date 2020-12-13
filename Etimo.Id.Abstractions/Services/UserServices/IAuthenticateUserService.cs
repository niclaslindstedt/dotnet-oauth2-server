using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthenticateUserService
    {
        Task<User> AuthenticateAsync(IAuthenticationRequest request);
        Task<User> AuthenticateAsync(string username, string password, string state = null);
    }
}
