using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthenticateUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }
}
