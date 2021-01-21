using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IImplicitTokenRequest
    {
        string Username    { get; }
        string Password    { get; }
        Guid   ClientId    { get; }
        string RedirectUri { get; }
        string Scope       { get; }
    }
}
