using Etimo.Id.Entities.Abstractions;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizeService
    {
        Task<string> AuthorizeAsync(IAuthorizationRequest request);
    }
}
