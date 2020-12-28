using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UpdateUserService : IUpdateUserService
    {
        private readonly IFindUserService _findUserService;
        private readonly IUserRepository  _userRepository;

        public UpdateUserService(IFindUserService findUserService, IUserRepository userRepository)
        {
            _findUserService = findUserService;
            _userRepository  = userRepository;
        }

        public async Task<User> UpdateAsync(User updatedUser)
        {
            User user = await _findUserService.FindAsync(updatedUser.UserId);

            user.Update(updatedUser);
            await _userRepository.SaveAsync();

            return user;
        }
    }
}
