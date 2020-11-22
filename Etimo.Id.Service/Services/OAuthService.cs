using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Etimo.Id.Service.Services
{
    public class OAuthService : IOAuthService
    {
        private readonly OAuthSettings _settings;
        private readonly IUsersService _usersService;

        public OAuthService(
            OAuthSettings settings,
            IUsersService usersService)
        {
            _settings = settings;
            _usersService = usersService;
        }

        public async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            if (request.GrantType == GrantTypes.Password)
            {
                var user = await _usersService.AuthenticateAsync(request.ClientId, request.ClientSecret);
                request.ClientId = user.UserId.ToString();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
                new Claim(ClaimTypes.NameIdentifier, request.ClientId),
                new Claim(ClaimTypes.Role, RoleNames.User),
                new Claim(ClaimTypes.Role, RoleNames.Admin)
            };

            var secretBytes = Encoding.UTF8.GetBytes(_settings.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _settings.Issuer,
                _settings.Audience,
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.TokenExpirationMinutes),
                signingCredentials);

            return CreateJwtToken(token);
        }

        private static JwtToken CreateJwtToken(SecurityToken token)
        {
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtToken
            {
                TokenBase64 = tokenJson,
                TokenType = TokenTypes.Bearer,
                ExpiresIn = (int)(token.ValidTo - DateTime.UtcNow).TotalSeconds
            };
        }
    }
}
