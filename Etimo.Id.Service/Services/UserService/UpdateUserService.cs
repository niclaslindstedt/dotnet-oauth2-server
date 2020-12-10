using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UpdateUserService : IUpdateUserService
    {
        private readonly IFindUserService _findUserService;
        private readonly IUserRepository _userRepository;

        public UpdateUserService(
            IFindUserService findUserService,
            IUserRepository userRepository)
        {
            _findUserService = findUserService;
            _userRepository = userRepository;
        }

        public async Task<User> UpdateAsync(User updatedUser)
        {
            var user = await _findUserService.FindAsync(updatedUser.UserId);

            user.Update(updatedUser);
            await _userRepository.SaveAsync();

            return user;
        }

        public Task<User> UpdateAsync(User updatedUser, Guid userId)
        {
            if (updatedUser.UserId != userId)
            {
                throw new BadRequestException("You can only update your own user.");
            }

            return UpdateAsync(updatedUser);
        }
    }
}
