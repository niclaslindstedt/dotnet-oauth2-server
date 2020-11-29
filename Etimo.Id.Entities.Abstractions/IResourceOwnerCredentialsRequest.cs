using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IResourceOwnerCredentialsRequest
    {
        Guid ClientId { get; }
        string ClientSecret { get; }
        string Username { get; }
        string Password { get; }
    }
}
