// ReSharper disable PossibleInterfaceMemberAmbiguity

namespace Etimo.Id.Entities.Abstractions
{
    public interface ITokenRequest
        : IUserCredentials,
            IAuthorizationCodeTokenRequest,
            IClientCredentialsTokenRequest,
            IImplicitTokenRequest,
            IResourceOwnerPasswordCredentialsTokenRequest,
            IRefreshTokenRequest
    {
        public string GrantType { get; }
    }
}
