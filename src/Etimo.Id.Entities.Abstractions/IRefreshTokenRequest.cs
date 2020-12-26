using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IRefreshTokenRequest
    {
        Guid ClientId { get; }
        string ClientSecret { get; }
        string RefreshToken { get; }
        string Scope { get; }
    }
}
