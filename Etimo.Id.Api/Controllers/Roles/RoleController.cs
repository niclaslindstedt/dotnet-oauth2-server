using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
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
        private readonly IRoleService _roleService;

        public RoleController(
            SiteSettings siteSettings,
            IRoleService roleService)
        {
            _siteSettings = siteSettings;
            _roleService = roleService;
        }

        [HttpGet]
        [Route("/roles")]
        [Authorize(Policy = RoleScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<Role> roles;
            if (this.UserHasScope(RoleScopes.Admin))
            {
                roles = await _roleService.GetAllAsync();
            }
            else
            {
                roles = await _roleService.GetByUserIdAsync(this.GetUserId());
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
                role = await _roleService.FindAsync(roleId);
            }
            else
            {
                role = await _roleService.FindAsync(roleId, this.GetUserId());
            }

            var found = RoleResponseDto.FromRole(role);

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
                role = await _roleService.AddAsync(dto.ToRole());
            }
            else
            {
                role = await _roleService.AddAsync(dto.ToRole(), this.GetUserId());
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
                role = await _roleService.UpdateAsync(dto.ToRole(roleId));
            }
            else
            {
                role = await _roleService.UpdateAsync(dto.ToRole(roleId), this.GetUserId());
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
                await _roleService.DeleteAsync(roleId);
            }
            else
            {
                await _roleService.DeleteAsync(roleId, this.GetUserId());
            }

            return NoContent();
        }
    }
}
