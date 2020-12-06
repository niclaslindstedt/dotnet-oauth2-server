using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationService
    {
        Task<string> AuthorizeAsync(AuthorizationRequest request);
        Task ValidateAsync(Guid accessTokenId);
    }
}
