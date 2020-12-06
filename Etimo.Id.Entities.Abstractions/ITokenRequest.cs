// ReSharper disable PossibleInterfaceMemberAmbiguity

namespace Etimo.Id.Entities.Abstractions
{
    public interface ITokenRequest
        : IClientCredentialsRequest,
            IUserCredentials,
            IAuthorizationCodeRequest,
            IResourceOwnerCredentialsRequest,
            IRefreshTokenRequest
    {
    }
}
