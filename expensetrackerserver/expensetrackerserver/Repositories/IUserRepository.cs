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

        Task<bool> EmailExists(string email);

        Task<bool> UsernameExists(string username);

    }
}
