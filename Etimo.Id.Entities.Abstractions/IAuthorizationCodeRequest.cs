using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthorizationCodeRequest
    {
        string Code { get; }
        Guid ClientId { get; }
        string ClientSecret { get; }
        string RedirectUri { get; }
    }
}
