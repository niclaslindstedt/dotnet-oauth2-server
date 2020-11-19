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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<TokenResponseDto> TokenAsync(TokenRequestDto req)
        {
            await ValidateRequestAsync(req);

            var token = GenerateToken(req.Username);
            var response = MapTokenResponse(token);

            return response;
        }

        private async Task ValidateRequestAsync(TokenRequestDto req)
        {
            // Grant access if no users in database -- we need someone to create those users!
            if (!await _context.Users.AnyAsync())
            {
                _logger.LogWarning("Granting access to user due to no users in database.");
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
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
