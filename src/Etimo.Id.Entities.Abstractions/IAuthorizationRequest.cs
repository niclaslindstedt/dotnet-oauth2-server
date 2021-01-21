namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthorizationRequest
        : IAuthenticationRequest,
            IImplicitTokenRequest
    {
        string ResponseType { get; }
        string Code         { get; }
    }
}
