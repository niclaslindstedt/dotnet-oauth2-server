using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Scopes;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Service.Scopes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Roles
{
    [ApiController]
    public class RoleController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IAddRoleScopeRelationService _addRoleScopeRelationService;
        private readonly IAddRoleService _addRoleService;
        private readonly IDeleteRoleScopeRelationService _deleteRoleScopeRelationService;
        private readonly IDeleteRoleService _deleteRoleService;
        private readonly IFindRoleService _findRoleService;
        private readonly IGetRolesService _getRolesService;
        private readonly IGetScopesService _getScopesService;
        private readonly IUpdateRoleService _updateRoleService;

        public RoleController(
            SiteSettings siteSettings,
            IAddRoleScopeRelationService addRoleScopeRelationService,
            IAddRoleService addRoleService,
            IDeleteRoleScopeRelationService deleteRoleScopeRelationService,
            IDeleteRoleService deleteRoleService,
            IFindRoleService findRoleService,
            IGetRolesService getRolesService,
            IGetScopesService getScopesService,
            IUpdateRoleService updateRoleService)
        {
            _siteSettings = siteSettings;
            _addRoleScopeRelationService = addRoleScopeRelationService;
            _addRoleService = addRoleService;
            _deleteRoleScopeRelationService = deleteRoleScopeRelationService;
            _deleteRoleService = deleteRoleService;
            _findRoleService = findRoleService;
            _getRolesService = getRolesService;
            _getScopesService = getScopesService;
            _updateRoleService = updateRoleService;
        }

        [HttpGet]
        [Route("/roles")]
        [Authorize(Policy = RoleScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<Role> roles;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                roles = await _getRolesService.GetAllAsync();
            }
            else
            {
                roles = await _getRolesService.GetByUserIdAsync(this.GetUserId());
            }

            var found = roles.Select(RoleResponseDto.FromRole);

            return Ok(found);
        }

        [HttpGet]
        [Route("/roles/{roleId:guid}")]
        [Authorize(Policy = RoleScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid roleId)
        {
            Role role;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                role = await _findRoleService.FindAsync(roleId);
            }
            else
            {
                role = await _findRoleService.FindAsync(roleId, this.GetUserId());
            }

            var found = RoleResponseDto.FromRole(role);

            return Ok(found);
        }

        [HttpGet]
        [Route("/roles/{roleId:guid}/scopes")]
        [Authorize(Policy = CombinedScopes.ReadRoleScope)]
        public async Task<IActionResult> GetScopesAsync([FromRoute] Guid roleId)
        {
            List<Scope> scopes;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                scopes = await _getScopesService.GetByRoleIdAsync(roleId);
            }
            else
            {
                scopes = await _getScopesService.GetByRoleIdAsync(roleId, this.GetUserId());
            }

            var found = scopes.Select(s => ScopeResponseDto.FromScope(s, false));

            return Ok(found);
        }

        [HttpPost]
        [Route("/roles")]
        [ValidateModel]
        [Authorize(Policy = RoleScopes.Write)]
        public async Task<IActionResult> CreateAsync([FromBody] RoleRequestDto dto)
        {
            Role role;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                role = await _addRoleService.AddAsync(dto.ToRole());
            }
            else
            {
                role = await _addRoleService.AddAsync(dto.ToRole(), this.GetUserId());
            }

            var created = RoleResponseDto.FromRole(role);

            return Created($"{_siteSettings.ListenUri}/roles/{role.RoleId}", created);
        }

        [HttpPut]
        [Route("/roles/{roleId:guid}")]
        [ValidateModel]
        [Authorize(Policy = RoleScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid roleId, [FromBody] RoleRequestDto dto)
        {
            Role role;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                role = await _updateRoleService.UpdateAsync(dto.ToRole(roleId));
            }
            else
            {
                role = await _updateRoleService.UpdateAsync(dto.ToRole(roleId), this.GetUserId());
            }

            var updated = RoleResponseDto.FromRole(role);

            return Ok(updated);
        }

        [HttpDelete]
        [Route("/roles/{roleId:guid}")]
        [Authorize(Policy = RoleScopes.Write)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid roleId)
        {
            if (this.UserHasScope(RoleScopes.Admin))
            {
                await _deleteRoleService.DeleteAsync(roleId);
            }
            else
            {
                await _deleteRoleService.DeleteAsync(roleId, this.GetUserId());
            }

            return NoContent();
        }

        [HttpPut]
        [Route("/roles/{roleId:guid}/scopes/{scopeId:guid}")]
        [Authorize(Policy = RoleScopes.Write)]
        public async Task<IActionResult> AddScopeRelationAsync([FromRoute] Guid roleId, [FromRoute] Guid scopeId)
        {
            Role role;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                role = await _addRoleScopeRelationService.AddScopeRelationAsync(roleId, scopeId);
            }
            else
            {
                role = await _addRoleScopeRelationService.AddScopeRelationAsync(roleId, scopeId, this.GetUserId());
            }

            var added = RoleResponseDto.FromRole(role);

            return Ok(added);
        }

        [HttpDelete]
        [Route("/roles/{roleId:guid}/scopes/{scopeId:guid}")]
        [Authorize(Policy = RoleScopes.Write)]
        public async Task<IActionResult> DeleteScopeRelationAsync([FromRoute] Guid roleId, [FromRoute] Guid scopeId)
        {
            Role role;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                role = await _deleteRoleScopeRelationService.DeleteScopeRelationAsync(roleId, scopeId);
            }
            else
            {
                role = await _deleteRoleScopeRelationService.DeleteScopeRelationAsync(roleId, scopeId, this.GetUserId());
            }

            var added = RoleResponseDto.FromRole(role);

            return Ok(added);
        }
    }
}
