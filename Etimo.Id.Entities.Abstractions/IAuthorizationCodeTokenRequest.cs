using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthorizationCodeTokenRequest
    {
        string Code { get; }
        Guid ClientId { get; }
        string ClientSecret { get; }
        string RedirectUri { get; }
        string Scope { get; set; }
    }
}
