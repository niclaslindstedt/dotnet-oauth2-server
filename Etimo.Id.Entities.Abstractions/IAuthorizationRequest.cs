using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthorizationRequest
    {
        string ResponseType { get; }
        Guid ClientId { get; }
        string Code { get; }
        string RedirectUri { get; }
        string Scope { get; }
        string State { get; }
        string Username { get; }
        string Password { get; }
    }
}
