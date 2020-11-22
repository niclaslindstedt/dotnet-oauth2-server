using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Security;
using Etimo.Id.Service.Settings;

namespace Etimo.Id.Service.Factories
{
    public class TokenGeneratorFactory : ITokenGeneratorFactory
    {
        private readonly IUsersService _usersService;
        private readonly OAuthSettings _settings;

        public TokenGeneratorFactory(
            IUsersService usersService,
            OAuthSettings settings)
        {
            _usersService = usersService;
            _settings = settings;
        }

        public ITokenGenerator Create(TokenRequest request)
        {
            return request.GrantType switch
            {
                GrantTypes.Password => new PasswordTokenGenerator(_usersService, _settings),
                _ => throw new BadRequestException("invalid_request")
            };
        }
    }
}
