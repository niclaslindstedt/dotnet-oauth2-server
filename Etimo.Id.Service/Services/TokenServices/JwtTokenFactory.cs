using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Etimo.Id.Service.TokenGenerators
{
    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly JwtSettings _settings;

        public JwtTokenFactory(JwtSettings settings)
        {
            _settings = settings;
        }

        public JwtToken CreateJwtToken(IJwtTokenRequest request)
        {
            var audiences = CompileAudiences(request.Audience);
            var expiresAt = DateTime.UtcNow.AddMinutes(_settings.LifetimeMinutes);
            var tokenId = Guid.NewGuid();

            var claims = new List<Claim>
            {
                // https://tools.ietf.org/html/rfc7519#section-4.1
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
                new Claim(JwtRegisteredClaimNames.Sub, request.Subject),
                new Claim(JwtRegisteredClaimNames.Aud, audiences, JsonClaimValueTypes.JsonArray),
                new Claim(JwtRegisteredClaimNames.Exp, GetUnixTime(expiresAt).ToString(), ClaimValueTypes.Integer32),
                new Claim(JwtRegisteredClaimNames.Nbf, GetUnixTime(DateTime.UtcNow.AddMinutes(-5)).ToString(), ClaimValueTypes.Integer32),
                new Claim(JwtRegisteredClaimNames.Iat, GetUnixTime(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer32),
                new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()),
                // https://tools.ietf.org/html/rfc7519#section-4.2
                new Claim(ClaimTypes.Role, RoleNames.User),
                new Claim(ClaimTypes.Role, RoleNames.Admin)
            };

            // https://tools.ietf.org/html/rfc8693#section-4.2
            if (request.Scope != null)
            {
                claims.Add(new Claim("scope", request.Scope));
            }

            var secretBytes = Encoding.UTF8.GetBytes(_settings.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtToken
            {
                TokenId = tokenId,
                AccessToken = tokenJson,
                TokenType = TokenTypes.Bearer,
                ExpiresIn = GetSecondsUntil(expiresAt),
                ExpiresAt = expiresAt,
                Scope = request.Scope
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
            var trimmedAudiences = nonEmptyAudiences.Select(a => a.Trim());
            return JsonSerializer.Serialize(trimmedAudiences);
        }

        private static int GetSecondsUntil(DateTime dateTime)
        {
            return GetSecondsBetween(DateTime.UtcNow, dateTime);
        }

        private static int GetUnixTime(DateTime dateTime)
        {
            return GetSecondsBetween(new DateTime(1970, 1, 1), dateTime);
        }

        private static int GetSecondsBetween(DateTime from, DateTime until)
        {
            return Math.Abs((int)Math.Ceiling((until - from).TotalSeconds));
        }
    }
}
