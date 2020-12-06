using Etimo.Id.Entities.Abstractions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationService
    {
        Task<string> AuthorizeAsync(IAuthorizationRequest request);
        Task ValidateAsync(Guid accessTokenId);
    }
}
