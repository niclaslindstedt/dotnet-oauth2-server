using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly ILockUserService _lockUserService;
        private readonly IPasswordHasher  _passwordHasher;
        private readonly IUserRepository  _userRepository;

        public AuthenticateUserService(
            IUserRepository userRepository,
            ILockUserService lockUserService,
            IPasswordHasher passwordHasher)
        {
            _userRepository  = userRepository;
            _lockUserService = lockUserService;
            _passwordHasher  = passwordHasher;
        }

        public Task<User> AuthenticateAsync(IAuthenticationRequest request)
            => AuthenticateAsync(request.Username, request.Password, request.State);

        public async Task<User> AuthenticateAsync(
            string username,
            string password,
            string state = null)
        {
            User user = await _userRepository.FindByUsernameAsync(username);
            if (user == null) { throw new InvalidGrantException("Invalid user credentials.", state); }

            if (user.IsLocked) { throw new InvalidGrantException("This user has been locked because of too many failed login attempts."); }

            if (!_passwordHasher.Verify(password, user.Password))
            {
                await _lockUserService.LockAsync(user);

                throw new InvalidGrantException("Invalid user credentials.", state);
            }

            user.FailedLogins = 0;
            await _userRepository.SaveAsync();

            return user;
        }
    }
}
