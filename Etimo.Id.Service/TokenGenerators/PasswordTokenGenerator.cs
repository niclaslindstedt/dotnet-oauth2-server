using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class PasswordTokenGenerator : IPasswordTokenGenerator
    {
        private readonly IUsersService _usersService;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public PasswordTokenGenerator(
            IUsersService usersService,
            IJwtTokenFactory jwtTokenFactory)
        {
            _usersService = usersService;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var user = await _usersService.AuthenticateAsync(request.ClientId, request.ClientSecret);
            request.ClientId = user.UserId.ToString();

            return _jwtTokenFactory.CreateJwtToken(request);
        }
    }
}
