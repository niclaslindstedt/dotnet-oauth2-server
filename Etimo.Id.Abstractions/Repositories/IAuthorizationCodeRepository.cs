using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationCodeRepository
    {
        void Add(AuthorizationCode code);
        ValueTask<AuthorizationCode> FindAsync(Guid codeId);
        Task<AuthorizationCode> FindByCodeAsync(string code);
        void RemoveRange(List<AuthorizationCode> codes);
        Task<int> SaveAsync();
    }
}
