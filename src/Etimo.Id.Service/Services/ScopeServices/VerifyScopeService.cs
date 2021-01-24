using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using System.Linq;

namespace Etimo.Id.Service
{
    public class VerifyScopeService : IVerifyScopeService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IScopeRepository       _scopeRepository;

        public VerifyScopeService(IScopeRepository scopeRepository, IApplicationRepository applicationRepository)
        {
            _scopeRepository       = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public bool Verify(string scope, User user)
        {
            if (scope == null) { return true; }

            string[] requestedScopes = scope.Split(" ");
            var      availableScopes = user.Roles.SelectMany(r => r.Scopes).Select(s => s.Name).ToList();

            if (!requestedScopes.All(s => availableScopes.Contains(s)))
            {
                throw new ForbiddenException("Invalid scope or insufficient access.");
            }

            return true;
        }
    }
}
