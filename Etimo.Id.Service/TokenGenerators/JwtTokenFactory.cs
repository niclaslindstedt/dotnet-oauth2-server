using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        public JwtToken CreateJwtToken(IJwtTokenRequest request)
        {
            var audiences = CompileAudiences(request.Audience);
            var expiresIn = GetSecondsUntil(DateTime.UtcNow.AddMinutes(_settings.TokenExpirationMinutes));
            var nowUnixTime = GetUnixTime(DateTime.UtcNow);

            var claims = new[]
            {
                // https://tools.ietf.org/html/rfc7519#section-4.1
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
                new Claim(JwtRegisteredClaimNames.Sub, request.Subject),
                new Claim(JwtRegisteredClaimNames.Aud, audiences),
                new Claim(JwtRegisteredClaimNames.Exp, expiresIn.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, nowUnixTime.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, nowUnixTime.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // https://tools.ietf.org/html/rfc7519#section-4.2
                new Claim(ClaimTypes.Role, Roles.User),
                new Claim(ClaimTypes.Role, Roles.Admin)
            };

            var secretBytes = Encoding.UTF8.GetBytes(_settings.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtToken
            {
                AccessToken = tokenJson,
                TokenType = TokenTypes.Bearer,
                ExpiresIn = (int)expiresIn
            };
        }

        private string CompileAudiences(List<string> audiences)
        {
            // We need to add this api as audience
            if (!audiences.Contains(_settings.Issuer))
            {
                audiences.Add(_settings.Issuer);
            }

            var nonEmptyAudiences = audiences.Where(a => !string.IsNullOrWhiteSpace(a));
            return string.Join(' ', nonEmptyAudiences.Select(a => a.Trim()));
        }

        private static long GetSecondsUntil(DateTime dateTime)
        {
            return GetSecondsBetween(DateTime.UtcNow, dateTime);
        }

        private static long GetUnixTime(DateTime dateTime)
        {
            return GetSecondsBetween(DateTime.MinValue, dateTime);
        }

        private static long GetSecondsBetween(DateTime from, DateTime until)
        {
            return Math.Abs((long)Math.Ceiling((until - from).TotalSeconds));
        }
    }
}
