using expensetrackerserver.DTOs;
using expensetrackerserver.Exceptions;
using expensetrackerserver.Repositories;

namespace expensetrackerserver.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        public AdminService(IAdminRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsers()
        {
            return await _repo.GetAllUsers();
        }

        public async Task<AdminUserDto> GetUserById(int userId)
        {
            var user = await _repo.GetUserById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return user;
        }

        public async Task DeactivateUser(int userId, int adminUserId)
        {
            if (userId == adminUserId)
            {
                throw new InvalidOperationException("Admin cannot deactivate their own account.");
            }
            var user = await _repo.GetUserById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User account is already deactivated.");
            }
            await _repo.DeactivateUser(userId, adminUserId);
        }

        public async Task ActivateUser(int userId)
        {
            var user = await _repo.GetUserById(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (user.IsActive)
            {
                throw new InvalidOperationException("User account is already active.");

            }
            await _repo.ActivateUser(userId);
        }

        public async Task DeleteUser(int userId, int adminUserId)
        {
            if (userId == adminUserId)
            {
                throw new InvalidOperationException("Admin cannot delete their own account.");
            }
            var user = await _repo.GetUserById(userId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }
            await _repo.DeleteUser(userId);
        }
    }
}
