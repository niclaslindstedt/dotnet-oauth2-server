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
        private readonly IUsersService _usersService;
        private readonly IApplicationsService _applicationsService;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public ResourceOwnerCredentialsTokenGenerator(
            IUsersService usersService,
            IApplicationsService applicationsService,
            IAccessTokensRepository accessTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _usersService = usersService;
            _applicationsService = applicationsService;
            _accessTokensRepository = accessTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IResourceOwnerCredentialsRequest request)
        {
            ValidateRequest(request);

            var user = await _usersService.AuthenticateAsync(request.Username, request.Password);
            var application = await _applicationsService.AuthenticateAsync(request.ClientId, request.ClientSecret);

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

        private static void ValidateRequest(IResourceOwnerCredentialsRequest request)
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
