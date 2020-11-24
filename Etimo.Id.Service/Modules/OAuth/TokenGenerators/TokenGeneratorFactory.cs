using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;

namespace Etimo.Id.Service.OAuth
{
    public class TokenGeneratorFactory : ITokenGeneratorFactory
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IUsersService _usersService;
        private readonly OAuthSettings _settings;

        public TokenGeneratorFactory(
            IApplicationsService applicationsService,
            IUsersService usersService,
            OAuthSettings settings)
        {
            _applicationsService = applicationsService;
            _usersService = usersService;
            _settings = settings;
        }

        public ITokenGenerator Create(TokenRequest request)
        {
            return request.GrantType switch
            {
                GrantTypes.ClientCredentials => new ClientCredentialsTokenGenerator(_applicationsService, _settings),
                GrantTypes.Password => new PasswordTokenGenerator(_usersService, _settings),
                _ => throw new InvalidRequestException("Grant type not supported.")
            };
        }
    }
}
