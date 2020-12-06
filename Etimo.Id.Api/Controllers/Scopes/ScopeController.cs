using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Scopes.Dtos;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Scopes
{
    [ApiController]
    public class ScopeController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IScopeService _scopeService;

        public ScopeController(
            SiteSettings siteSettings,
            IScopeService scopeService)
        {
            _siteSettings = siteSettings;
            _scopeService = scopeService;
        }

        [HttpGet]
        [Route("/scopes")]
        [Authorize(Policy = ScopeScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<Scope> scopes;
            if (this.UserHasScope(ScopeScopes.Admin))
            {
                scopes = await _scopeService.GetAllAsync();
            }
            else
            {
                scopes = await _scopeService.GetByClientIdAsync(this.GetClientId());
            }

            return Ok(scopes.Select(ScopeResponseDto.FromScope));
        }

        [HttpGet]
        [Route("/scopes/{scopeId:guid}")]
        [Authorize(Policy = ScopeScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid scopeId)
        {
            Scope scope;
            if (this.UserHasScope(ScopeScopes.Admin))
            {
                scope = await _scopeService.FindAsync(scopeId);
            }
            else
            {
                scope = await _scopeService.FindAsync(scopeId, this.GetUserId());
            }

            return Ok(ScopeResponseDto.FromScope(scope));
        }

        [HttpPost]
        [Route("/scopes")]
        [ValidateModel]
        [Authorize(Policy = ScopeScopes.Write)]
        public async Task<IActionResult> CreateAsync([FromBody] ScopeRequestDto dto)
        {
            var scope = await _scopeService.AddAsync(dto.ToScope(), this.GetUserId());
            var created = ScopeResponseDto.FromScope(scope);

            return Created($"{_siteSettings.ListenUri}/scopes/{scope.ApplicationId}", created);
        }

        [HttpPut]
        [Route("/scopes/{scopeId:guid}")]
        [ValidateModel]
        [Authorize(Policy = ScopeScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid scopeId, [FromBody] ScopeRequestDto dto)
        {
            var scope = await _scopeService.UpdateAsync(dto.ToScope(scopeId), this.GetUserId());
            var created = ScopeResponseDto.FromScope(scope);

            return Ok(created);
        }

        [HttpDelete]
        [Route("/scopes/{scopeId:guid}")]
        [Authorize(Policy = ScopeScopes.Write)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid scopeId)
        {
            if (this.UserHasScope(ScopeScopes.Admin))
            {
                await _scopeService.DeleteAsync(scopeId);
            }
            else
            {
                await _scopeService.DeleteAsync(scopeId, this.GetUserId());
            }

            return NoContent();
        }
    }
}
