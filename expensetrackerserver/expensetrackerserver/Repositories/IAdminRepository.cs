using expensetrackerserver.DTOs;

namespace expensetrackerserver.Repositories
{
    public interface IAdminRepository
    {

        Task<IEnumerable<AdminUserDto>> GetAllUsers();

        Task<AdminUserDto?> GetUserById(int userId);

        Task DeactivateUser(int userId, int adminUserId);

        Task ActivateUser(int userId);

        Task DeleteUser(int userId);
    }
}
