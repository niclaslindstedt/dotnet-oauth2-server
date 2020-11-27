namespace Etimo.Id.Entities.Abstractions
{
    public interface IClientCredentialsRequest
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
}
