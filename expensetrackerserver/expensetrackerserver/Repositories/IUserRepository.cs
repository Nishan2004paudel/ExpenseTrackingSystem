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

    }
}
