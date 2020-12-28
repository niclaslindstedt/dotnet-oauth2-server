using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthorizationRequest : IAuthenticationRequest
    {
        string ResponseType { get; }
        Guid   ClientId     { get; }
        string Code         { get; }
        string RedirectUri  { get; }
        string Scope        { get; }
    }
}
