namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthorizationCodeRequest
    {
        string Code { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string RedirectUri { get; }
    }
}
