using expensetrackerserver.DTOs;
using expensetrackerserver.Models;

namespace expensetrackerserver.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDetailDto>> GetAll();
        Task<User?> GetByEmailOrUsername(string identifier);
        Task<int> Create(User user);

        Task<User?> GetById(int id);
        Task IncrementTokenVersion(int userId);

        Task<bool> EmailExists(string email);

        Task<bool> UsernameExists(string username);
        Task<int?> GetTokenVersion(int userId);
        Task<User?> GetByEmailVerificationToken(string token);
        Task VerifyEmail(int userId);

        Task<User?> GetByEmail(string email);
        Task UpdateEmailVerificationToken(int userId, string token, DateTime expiresAt);
        Task UpdateEmail(int userId, string email);

    }
}
