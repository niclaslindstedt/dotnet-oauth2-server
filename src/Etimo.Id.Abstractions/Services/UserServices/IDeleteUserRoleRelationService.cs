using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteUserRoleRelationService
    {
        Task<List<Role>> DeleteRoleRelationAsync(Guid userId, Guid roleId);
    }
}
