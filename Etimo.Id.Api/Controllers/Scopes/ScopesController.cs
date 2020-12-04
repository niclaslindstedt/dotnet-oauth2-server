using System;
using System.Linq;
using Etimo.Id.Abstractions;
using Etimo.Id.Api.Applications;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Security;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Etimo.Id.Api.Scopes.Dtos;

namespace Etimo.Id.Api.Scopes
{
    [ApiController]
    public class ScopesController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IScopesService _scopesService;

        public ScopesController(
            SiteSettings siteSettings,
            IScopesService scopesService)
        {
            _siteSettings = siteSettings;
            _scopesService = scopesService;
        }

        [HttpGet]
        [Route("/scopes")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetAsync()
        {
            // If the user calling is not an admin, revert to the GetByClientIdAsync method.
            if (!this.UserHasRole(Roles.Admin))
            {
                var scopes = await _scopesService.GetByClientIdAsync(this.GetClientId());
                return Ok(scopes);
            }

            var allScopes = await _scopesService.GetAllAsync();

            return Ok(allScopes.Select(ScopeResponseDto.FromScope));
        }

        [HttpGet]
        [Route("/scopes/{scopeId:guid}")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid scopeId)
        {
            Scope scope;
            if (this.UserHasRole(Roles.Admin))
            {
                scope = await _scopesService.FindAsync(scopeId);
            }
            else
            {
                scope = await _scopesService.FindAsync(scopeId, this.GetUserId());
            }

            return Ok(ScopeResponseDto.FromScope(scope));
        }

        [HttpPost]
        [Route("/scopes")]
        [ValidateModel]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> CreateAsync([FromBody] ScopeRequestDto dto)
        {
            var scope = await _scopesService.AddAsync(dto.ToScope(), this.GetUserId());
            var created = ScopeResponseDto.FromScope(scope);

            return Created($"{_siteSettings.ListenUri}/applications/{scope.ApplicationId}", created);
        }

        [HttpPut]
        [Route("/scopes/{scopeId:guid}")]
        [ValidateModel]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid scopeId, [FromBody] ScopeRequestDto dto)
        {
            var scope = await _scopesService.UpdateAsync(dto.ToScope(scopeId), this.GetUserId());
            var created = ScopeResponseDto.FromScope(scope);

            return Ok(created);
        }

        [HttpDelete]
        [Route("/scopes/{scopeId:guid}")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid scopeId)
        {
            if (this.UserHasRole(Roles.Admin))
            {
                await _scopesService.DeleteAsync(scopeId);
            }

            await _scopesService.DeleteAsync(scopeId, this.GetUserId());

            return NoContent();
        }
    }
}
