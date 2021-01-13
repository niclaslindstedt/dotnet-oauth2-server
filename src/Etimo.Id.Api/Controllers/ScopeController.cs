using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Dtos;
using Etimo.Id.Entities;
using Etimo.Id.Security;
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
        private readonly IAddScopeService    _addScopeService;
        private readonly IDeleteScopeService _deleteScopeService;
        private readonly IFindScopeService   _findScopeService;
        private readonly IGetRolesService    _getRolesService;
        private readonly IGetScopesService   _getScopesService;
        private readonly SiteSettings        _siteSettings;
        private readonly IUpdateScopeService _updateScopeService;

        public ScopeController(
            SiteSettings siteSettings,
            IAddScopeService addScopeService,
            IDeleteScopeService deleteScopeService,
            IFindScopeService findScopeService,
            IGetRolesService getRolesService,
            IGetScopesService getScopesService,
            IUpdateScopeService updateScopeService)
        {
            _siteSettings       = siteSettings;
            _addScopeService    = addScopeService;
            _deleteScopeService = deleteScopeService;
            _findScopeService   = findScopeService;
            _getRolesService    = getRolesService;
            _getScopesService   = getScopesService;
            _updateScopeService = updateScopeService;
        }

        [HttpGet]
        [Route("/scopes")]
        [Authorize(Policy = ScopeScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<Scope> scopes;
            if (this.UserHasScope(ScopeScopes.Admin)) { scopes = await _getScopesService.GetAllAsync(); }
            else { scopes                                      = await _getScopesService.GetByClientIdAsync(this.GetClientId()); }

            IEnumerable<ScopeResponseDto> found = scopes.Select(ScopeResponseDto.FromScope);

            return Ok(found);
        }

        [HttpGet]
        [Route("/scopes/{scopeId:guid}")]
        [Authorize(Policy = ScopeScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid scopeId)
        {
            Scope scope;
            if (this.UserHasScope(ScopeScopes.Admin)) { scope = await _findScopeService.FindAsync(scopeId); }
            else { scope                                      = await _findScopeService.FindAsync(scopeId, this.GetUserId()); }

            var found = ScopeResponseDto.FromScope(scope);

            return Ok(found);
        }

        [HttpGet]
        [Route("/scopes/{scopeId:guid}/roles")]
        [Authorize(Policy = CombinedScopes.ReadRoleScope)]
        public async Task<IActionResult> FindRolesAsync([FromRoute] Guid scopeId)
        {
            List<Role> roles;
            if (this.UserHasScope(ScopeScopes.Admin)) { roles = await _getRolesService.GetByScopeIdAsync(scopeId); }
            else { roles                                      = await _getRolesService.GetByScopeIdAsync(scopeId, this.GetUserId()); }

            IEnumerable<RoleResponseDto> found = roles.Select(r => RoleResponseDto.FromRole(r, false));

            return Ok(found);
        }

        [HttpPost]
        [Route("/scopes")]
        [ValidateModel]
        [Authorize(Policy = ScopeScopes.Write)]
        public async Task<IActionResult> CreateAsync([FromBody] ScopeRequestDto dto)
        {
            Scope scope;
            if (this.UserHasScope(ScopeScopes.Admin)) { scope = await _addScopeService.AddAsync(dto.ToScope()); }
            else { scope                                      = await _addScopeService.AddAsync(dto.ToScope(), this.GetUserId()); }

            var created = ScopeResponseDto.FromScope(scope);

            return Created($"{_siteSettings.ListenUri}/scopes/{scope.ScopeId}", created);
        }

        [HttpPut]
        [Route("/scopes/{scopeId:guid}")]
        [ValidateModel]
        [Authorize(Policy = ScopeScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid scopeId, [FromBody] ScopeRequestDto dto)
        {
            Scope scope;
            if (this.UserHasScope(ScopeScopes.Admin)) { scope = await _updateScopeService.UpdateAsync(dto.ToScope(scopeId)); }
            else { scope = await _updateScopeService.UpdateAsync(dto.ToScope(scopeId), this.GetUserId()); }

            var updated = ScopeResponseDto.FromScope(scope);

            return Ok(updated);
        }

        [HttpDelete]
        [Route("/scopes/{scopeId:guid}")]
        [Authorize(Policy = ScopeScopes.Write)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid scopeId)
        {
            if (this.UserHasScope(ScopeScopes.Admin)) { await _deleteScopeService.DeleteAsync(scopeId); }
            else { await _deleteScopeService.DeleteAsync(scopeId, this.GetUserId()); }

            return NoContent();
        }
    }
}
