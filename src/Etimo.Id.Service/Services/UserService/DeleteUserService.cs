using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteUserService : IDeleteUserService
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task DeleteAsync(Guid userId)
        {
            User user = await _userRepository.FindAsync(userId);
            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveAsync();
            }
        }
    }
}
