using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ResourceOwnerCredentialsTokenGenerator : IResourceOwnerCredentialsTokenGenerator
    {
        private readonly IUsersService _usersService;
        private readonly IApplicationsService _applicationsService;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public ResourceOwnerCredentialsTokenGenerator(
            IUsersService usersService,
            IApplicationsService applicationsService,
            IJwtTokenFactory jwtTokenFactory)
        {
            _usersService = usersService;
            _applicationsService = applicationsService;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IResourceOwnerCredentialsRequest request)
        {
            var user = await _usersService.AuthenticateAsync(request.Username, request.Password);
            var application = await _applicationsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { application.HomepageUri },
                Subject = user.UserId.ToString()
            };

            return _jwtTokenFactory.CreateJwtToken(jwtRequest);
        }
    }
}
