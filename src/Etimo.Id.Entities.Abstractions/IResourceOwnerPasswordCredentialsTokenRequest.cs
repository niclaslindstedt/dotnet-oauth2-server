using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IResourceOwnerPasswordCredentialsTokenRequest
    {
        Guid   ClientId          { get; }
        string ClientSecret      { get; }
        string Username          { get; }
        string Password          { get; }
        string Scope             { get; }
        bool   CredentialsInBody { get; }
    }
}
