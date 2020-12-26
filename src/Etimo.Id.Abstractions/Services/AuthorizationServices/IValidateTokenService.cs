using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IValidateTokenService
    {
        Task ValidateTokenAsync(Guid accessTokenId);
    }
}
