using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IGetScopesService
    {
        Task<List<Scope>> GetAllAsync();
        Task<List<Scope>> GetByClientIdAsync(Guid clientId);
    }
}
