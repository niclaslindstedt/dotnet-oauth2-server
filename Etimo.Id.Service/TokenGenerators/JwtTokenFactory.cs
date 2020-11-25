using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Etimo.Id.Service.TokenGenerators
{
    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly OAuthSettings _settings;

        public JwtTokenFactory(OAuthSettings settings)
        {
            _settings = settings;
        }

        public JwtToken CreateJwtToken(TokenRequest request)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
                new Claim(ClaimTypes.NameIdentifier, request.ClientId),
                new Claim(ClaimTypes.Role, Roles.User),
                new Claim(ClaimTypes.Role, Roles.Admin)
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
