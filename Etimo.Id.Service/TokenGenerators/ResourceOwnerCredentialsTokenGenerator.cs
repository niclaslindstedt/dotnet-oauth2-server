using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ResourceOwnerCredentialsTokenGenerator : IResourceOwnerCredentialsTokenGenerator
    {
        private readonly IUserService _userService;
        private readonly IApplicationService _applicationService;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public ResourceOwnerCredentialsTokenGenerator(
            IUserService userService,
            IApplicationService applicationService,
            IAccessTokensRepository accessTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _userService = userService;
            _applicationService = applicationService;
            _accessTokensRepository = accessTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IResourceOwnerPasswordCredentialsTokenRequest request)
        {
            ValidateRequest(request);

            var user = await _userService.AuthenticateAsync(request.Username, request.Password);
            var application = await _applicationService.AuthenticateAsync(request.ClientId, request.ClientSecret);

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { application.ClientId.ToString() },
                Subject = user.UserId.ToString()
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);

            _accessTokensRepository.Add(jwtToken.ToAccessToken());
            await _accessTokensRepository.SaveAsync();

            return jwtToken;
        }

        private static void ValidateRequest(IResourceOwnerPasswordCredentialsTokenRequest request)
        {
            if (request.ClientId == Guid.Empty || request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (request.Username == null || request.Password == null)
            {
                throw new InvalidGrantException("Invalid resource owner credentials.");
            }
        }
    }
}
