using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationCodeRepository
    {
        void Add(AuthorizationCode code);
        Task<int> SaveAsync();
        ValueTask<AuthorizationCode> FindAsync(Guid codeId);
        void RemoveRange(List<AuthorizationCode> codes);
    }
}
