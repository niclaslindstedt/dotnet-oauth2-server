// ReSharper disable PossibleInterfaceMemberAmbiguity

namespace Etimo.Id.Entities.Abstractions
{
    public interface ITokenRequest
        : IUserCredentials,
            IAuthorizationCodeTokenRequest,
            IClientCredentialsTokenRequest,
            IResourceOwnerPasswordCredentialsTokenRequest,
            IRefreshTokenRequest
    {
        public string GrantType { get; }
    }
}
