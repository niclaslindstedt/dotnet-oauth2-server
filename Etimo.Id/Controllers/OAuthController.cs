using Etimo.Id.Constants;
using Etimo.Id.Models;
using Etimo.Id.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Etimo.Id.Exceptions;

namespace Etimo.Id.Controllers
{
    [ApiController]
    [Route("oauth2")]
    public class OAuthController : Controller
    {
        private readonly OAuthSettings _settings;
        private readonly ILogger<OAuthController> _logger;
        private readonly EtimoIdDbContext _context;

        public OAuthController(
            OAuthSettings settings,
            ILogger<OAuthController> logger,
            EtimoIdDbContext context)
        {
            _settings = settings;
            _logger = logger;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("validate")]
        public void Validate()
        {
        }

        [HttpPost]
        [Route("token")]
        public async Task<TokenResponseDto> TokenAsync([FromForm] TokenRequestForm req)
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                GetCredentialsFromAuthorizationHeader(req);
            }

            await ValidateRequestAsync(req);

            var token = GenerateToken(req.client_id);
            var response = MapTokenResponse(token);

            return response;
        }

        private void GetCredentialsFromAuthorizationHeader(TokenRequestForm req)
        {
            try
            {
                // Basic authentication -- https://tools.ietf.org/html/rfc2617
                // Authorization: Basic <base64 encoded string of "username:password">
                var header = Request.Headers["Authorization"];
                var parts = header.First().Split(' ');
                var type = parts[0].ToLowerInvariant();
                if (type != "basic") return;
                _logger.LogDebug("Logging in with basic auth");
                var authBytes = Convert.FromBase64String(parts[1]);
                var authString = Encoding.UTF8.GetString(authBytes);
                var authParts = authString.Split(':');
                req.client_id = authParts[0];
                req.client_secret = authParts[1];
            }
            catch (Exception ex)
            {
                _logger.LogError("Invalid basic authentication headers", ex);
                throw new BadRequestException("invalid_request");
            }
        }

        private async Task ValidateRequestAsync(TokenRequestForm req)
        {
            // Grant access if no users in database -- we need someone to create those users!
            if (!await _context.Users.AnyAsync())
            {
                _logger.LogWarning("Granting access to user due to no users in database.");
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.client_id);
            if (user == null)
            {
                throw new BadHttpRequestException("invalid_grant");
            }
        }

        private JwtSecurityToken GenerateToken(string clientId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, clientId),
                new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
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

            return token;
        }

        private static TokenResponseDto MapTokenResponse(SecurityToken token)
        {
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseDto
            {
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                JwtToken = tokenJson,
                ExpiresIn = (int)(token.ValidTo - DateTime.UtcNow).TotalSeconds
            };
        }
    }
}
