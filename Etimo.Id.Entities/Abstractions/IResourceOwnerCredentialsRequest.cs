namespace Etimo.Id.Entities.Abstractions
{
    public interface IResourceOwnerCredentialsRequest
    {
        string ClientId { get; }
        string ClientSecret { get; }
        string Username { get; }
        string Password { get; }
    }
}
