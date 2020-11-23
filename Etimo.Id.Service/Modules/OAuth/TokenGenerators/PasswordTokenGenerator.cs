using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Service.OAuth
{
    internal class PasswordTokenGenerator : TokenGenerator
    {
        private readonly IUsersService _usersService;

        public PasswordTokenGenerator(
            IUsersService usersService,
            OAuthSettings settings) : base(settings)
        {
            _usersService = usersService;
        }

        public override async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var user = await _usersService.AuthenticateAsync(request.ClientId, request.ClientSecret);
            request.ClientId = user.UserId.ToString();

            return CreateJwtToken(request);
        }
    }
}
