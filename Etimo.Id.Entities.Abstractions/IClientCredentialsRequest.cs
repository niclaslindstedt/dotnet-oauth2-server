using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IClientCredentialsRequest
    {
        Guid ClientId { get; }
        string ClientSecret { get; }
    }
}
